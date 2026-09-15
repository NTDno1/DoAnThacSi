// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: RAG-SQL Engine & Safety Guard
// ============================================================

using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AIBaseFramework.AI.Gateway;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.SQL;

// ============================================================
// RAG-SQL ENGINE
// ============================================================

/// <summary>
/// SQL Query request
/// </summary>
public class SQLQueryRequest
{
    public string NaturalLanguageQuery { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public int MaxRows { get; set; } = 100;
    public bool ExplainOnly { get; set; } = false;
}

/// <summary>
/// SQL Query response
/// </summary>
public class SQLQueryResponse
{
    public bool Success { get; set; }
    public string? GeneratedSQL { get; set; }
    public string? Error { get; set; }
    public DataTable? Results { get; set; }
    public int RowCount { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public List<string> Columns { get; set; } = new();
    public string FormattedResults { get; set; } = string.Empty;
    public List<Dictionary<string, object?>> Rows { get; set; } = new();
}

/// <summary>
/// SQL Engine interface
/// </summary>
public interface ISQLEngine
{
    Task<SQLQueryResponse> QueryAsync(SQLQueryRequest request, CancellationToken cancellationToken = default);
    Task<string> GenerateSQLAsync(string naturalLanguageQuery, string? userId = null, CancellationToken cancellationToken = default);
    Task<bool> ValidateAndExplainAsync(string sql, CancellationToken cancellationToken = default);
    List<string> GetAvailableTables();
    string GetSchemaDescription();
}

/// <summary>
/// RAG-SQL Engine implementation
/// </summary>
public class RAGSQLEngine : ISQLEngine
{
    private readonly IAI Gateway _aiGateway;
    private readonly IDbContextFactory<AI DbContext> _dbContextFactory;
    private readonly ISQLSchemaProvider _schemaProvider;
    private readonly ILogger<RAGSQLEngine> _logger;
    private readonly SQLEngineConfig _config;
    
    public RAGSQLEngine(
        IAI Gateway aiGateway,
        IDbContextFactory<AI DbContext> dbContextFactory,
        ISQLSchemaProvider schemaProvider,
        ILogger<RAGSQLEngine> logger,
        SQLEngineConfig config)
    {
        _aiGateway = aiGateway;
        _dbContextFactory = dbContextFactory;
        _schemaProvider = schemaProvider;
        _logger = logger;
        _config = config;
    }
    
    /// <summary>
    /// Execute natural language query
    /// </summary>
    public async Task<SQLQueryResponse> QueryAsync(SQLQueryRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new SQLQueryResponse();
        
        try
        {
            _logger.LogInformation("Executing SQL query from NL: {Query}", request.NaturalLanguageQuery);
            
            // Step 1: Generate SQL from natural language
            var sql = await GenerateSQLAsync(request.NaturalLanguageQuery, request.UserId, cancellationToken);
            response.GeneratedSQL = sql;
            
            if (request.ExplainOnly)
            {
                response.Success = true;
                response.ExecutionTime = stopwatch.Elapsed;
                return response;
            }
            
            // Step 2: Validate SQL (safety check)
            if (!await ValidateAndExplainAsync(sql, cancellationToken))
            {
                response.Success = false;
                response.Error = "SQL validation failed - potential security issue";
                return response;
            }
            
            // Step 3: Execute SQL
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            
            var results = await context.Database
                .SqlQueryRaw<object[]>(sql)
                .Take(request.MaxRows)
                .ToListAsync(cancellationToken);
            
            // Step 4: Format results
            response.Success = true;
            response.RowCount = results.Count;
            response.ExecutionTime = stopwatch.Elapsed;
            
            // Get column info from first result
            if (results.Any())
            {
                var props = results[0].GetType().GetProperties();
                response.Columns = props.Select(p => p.Name).ToList();
                
                foreach (var row in results)
                {
                    var dict = new Dictionary<string, object?>();
                    for (int i = 0; i < props.Length; i++)
                    {
                        dict[props[i].Name] = props[i].GetValue(row);
                    }
                    response.Rows.Add(dict);
                }
            }
            
            response.FormattedResults = FormatResults(response.Columns, response.Rows);
            
            _logger.LogInformation("SQL query executed: {RowCount} rows in {Time}ms", 
                response.RowCount, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQL query failed");
            response.Success = false;
            response.Error = ex.Message;
            response.ExecutionTime = stopwatch.Elapsed;
        }
        
        return response;
    }
    
    /// <summary>
    /// Generate SQL from natural language
    /// </summary>
    public async Task<string> GenerateSQLAsync(
        string naturalLanguageQuery, 
        string? userId = null, 
        CancellationToken cancellationToken = default)
    {
        var schema = _schemaProvider.GetSchemaDescription();
        
        var systemPrompt = $@"You are a SQL query generator. Generate PostgreSQL queries based on natural language input.

DATABASE SCHEMA:
{schema}

RULES:
1. Only SELECT queries are allowed (no INSERT, UPDATE, DELETE)
2. Always include tenant_id filter for multi-tenant tables: WHERE tenant_id = 'current_tenant'
3. Use proper JOIN syntax
4. Use table aliases for clarity
5. Limit results to 100 rows by default
6. Use ILIKE for case-insensitive text search

Return ONLY the SQL query, no explanation. If query cannot be generated, return: SELECT 'Cannot generate query' AS error";

        var response = await _aiGateway.ChatAsync(new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = systemPrompt },
                new() { Role = MessageRole.User, Content = naturalLanguageQuery }
            },
            Options = new AIRequestOptions
            {
                Temperature = 0.1, // Low temperature for deterministic SQL
                MaxTokens = 500,
                UserId = userId
            }
        }, cancellationToken);
        
        return CleanSQLResponse(response.Content);
    }
    
    /// <summary>
    /// Validate SQL query
    /// </summary>
    public async Task<bool> ValidateAndExplainAsync(string sql, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sql) || sql.Contains("';", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        
        // Basic SQL injection prevention
        var dangerousPatterns = new[]
        {
            "DROP ", "DELETE ", "TRUNCATE ", "ALTER ", "CREATE ", "INSERT ", "UPDATE ",
            "--", "/*", "*/", ";--", "xp_", "sp_", "exec", "execute"
        };
        
        var upperSql = sql.ToUpperInvariant();
        foreach (var pattern in dangerousPatterns)
        {
            if (upperSql.Contains(pattern))
            {
                _logger.LogWarning("SQL validation failed: dangerous pattern detected: {Pattern}", pattern);
                return false;
            }
        }
        
        // Use EXPLAIN to validate query syntax
        try
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Database
                .SqlQueryRaw<object>($"EXPLAIN {sql}")
                .Take(1)
                .ToListAsync(cancellationToken);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SQL validation failed");
            return false;
        }
    }
    
    public List<string> GetAvailableTables()
    {
        return _schemaProvider.GetAvailableTables();
    }
    
    public string GetSchemaDescription()
    {
        return _schemaProvider.GetSchemaDescription();
    }
    
    private static string CleanSQLResponse(string content)
    {
        // Remove markdown code blocks
        content = content.Trim();
        if (content.StartsWith("```sql"))
            content = content.Substring(6);
        if (content.StartsWith("```"))
            content = content.Substring(3);
        if (content.EndsWith("```"))
            content = content.Substring(0, content.Length - 3);
        
        return content.Trim();
    }
    
    private static string FormatResults(List<string> columns, List<Dictionary<string, object?>> rows)
    {
        if (!rows.Any()) return "No results found.";
        
        var sb = new System.Text.StringBuilder();
        
        // Header
        sb.AppendLine("┌" + string.Join(" │ ", columns.Select(c => PadCenter(c, 20))) + "┐");
        sb.AppendLine("├" + string.Join("─┼─", columns.Select(_ => new string('─', 20))) + "┤");
        
        // Rows
        foreach (var row in rows.Take(10))
        {
            sb.AppendLine("│" + string.Join(" │ ", columns.Select(c => PadCenter(
                row.GetValueOrDefault(c)?.ToString() ?? "NULL", 20))) + "│");
        }
        
        if (rows.Count > 10)
        {
            sb.AppendLine($"└{"─".Repeat(20 * columns.Count + columns.Count - 1)}┘");
            sb.AppendLine($"... and {rows.Count - 10} more rows");
        }
        else
        {
            sb.AppendLine("└" + string.Join("─┴─", columns.Select(_ => new string('─', 20))) + "┘");
        }
        
        return sb.ToString();
    }
    
    private static string PadCenter(string text, int width)
    {
        if (text.Length >= width) return text.Substring(0, width);
        var pad = width - text.Length;
        return new string(' ', pad / 2) + text + new string(' ', pad - pad / 2);
    }
}

// ============================================================
// SQL SCHEMA PROVIDER
// ============================================================

public interface ISQLSchemaProvider
{
    string GetSchemaDescription();
    List<string> GetAvailableTables();
    Dictionary<string, List<ColumnInfo>> GetTableColumns();
}

public class ColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
}

public class DefaultSQLSchemaProvider : ISQLSchemaProvider
{
    private readonly string _schemaDescription;
    private readonly List<string> _tables;
    
    public DefaultSQLSchemaProvider()
    {
        _tables = new List<string>
        {
            "users", "documents", "document_chunks", "chat_sessions", 
            "chat_messages", "search_logs", "audit_logs", "system_settings"
        };
        
        _schemaDescription = @"
TABLE: users
- id (UUID, PK)
- email (VARCHAR)
- full_name (VARCHAR)
- role (VARCHAR)
- department (VARCHAR)
- tenant_id (UUID)
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)

TABLE: documents
- id (UUID, PK)
- title (VARCHAR)
- description (TEXT)
- file_path (VARCHAR)
- file_type (VARCHAR)
- status (VARCHAR)
- owner_id (UUID, FK -> users.id)
- tenant_id (UUID)
- created_at (TIMESTAMP)

TABLE: document_chunks
- id (UUID, PK)
- document_id (UUID, FK -> documents.id)
- content (TEXT)
- chunk_index (INT)
- embedding (VECTOR(768))
- tenant_id (UUID)
- created_at (TIMESTAMP)

TABLE: chat_sessions
- id (UUID, PK)
- user_id (UUID, FK -> users.id)
- title (VARCHAR)
- message_count (INT)
- tenant_id (UUID)
- created_at (TIMESTAMP)

TABLE: chat_messages
- id (UUID, PK)
- session_id (UUID, FK -> chat_sessions.id)
- role (VARCHAR)
- content (TEXT)
- citations (JSONB)
- tenant_id (UUID)
- created_at (TIMESTAMP)
";
    }
    
    public string GetSchemaDescription() => _schemaDescription;
    public List<string> GetAvailableTables() => _tables;
    public Dictionary<string, List<ColumnInfo>> GetTableColumns() => new();
}

public class SQLEngineConfig
{
    public bool AllowDML { get; set; } = false; // Only SELECT
    public int MaxResultRows { get; set; } = 100;
    public int QueryTimeoutSeconds { get; set; } = 30;
    public bool EnableExplainPlan { get; set; } = true;
}

// ============================================================
// AI SAFETY GUARD
// ============================================================

/// <summary>
/// Safety check result
/// </summary>
public class SafetyCheckResult
{
    public bool IsAllowed { get; set; }
    public string? Reason { get; set; }
    public List<string> Warnings { get; set; } = new();
    public double RiskScore { get; set; }
}

/// <summary>
/// Safety Guard interface
/// </summary>
public interface IAgentSafetyGuard
{
    Task<SafetyCheckResult> CheckActionAsync(AgentAction action, AgentSession session, CancellationToken cancellationToken = default);
    Task<SafetyCheckResult> CheckPromptAsync(string prompt, AgentSession session, CancellationToken cancellationToken = default);
    Task<SafetyCheckResult> CheckSQLAsync(string sql, AgentSession session, CancellationToken cancellationToken = default);
}

/// <summary>
/// Safety Guard implementation
/// </summary>
public class AgentSafetyGuard : IAgentSafetyGuard
{
    private readonly IAI Gateway _aiGateway;
    private readonly ILogger<AgentSafetyGuard> _logger;
    private readonly SafetyGuardConfig _config;
    
    // Dangerous SQL patterns
    private static readonly string[] _dangerousSQLPatterns = new[]
    {
        "DROP ", "DELETE ", "TRUNCATE ", "ALTER ", "CREATE ", "INSERT ", "UPDATE ",
        "GRANT ", "REVOKE ", "EXEC ", "EXECUTE ", "xp_", "sp_", "--", "/*", "*/"
    };
    
    // Sensitive keywords
    private static readonly string[] _sensitiveKeywords = new[]
    {
        "password", "secret", "key", "token", "credential", "ssn", "credit card"
    };
    
    // Approved action types
    private static readonly HashSet<string> _approvedActions = new(StringComparer.OrdinalIgnoreCase)
    {
        "SearchDocuments", "GenerateResponse", "RetrieveContext", "ExecuteSQLQuery",
        "CreateEntity", "UpdateEntity", "DeleteEntity", "ExecuteWorkflow"
    };
    
    public AgentSafetyGuard(IAI Gateway aiGateway, ILogger<AgentSafetyGuard> logger, SafetyGuardConfig config)
    {
        _aiGateway = aiGateway;
        _logger = logger;
        _config = config;
    }
    
    /// <summary>
    /// Check if an action is safe to execute
    /// </summary>
    public async Task<SafetyCheckResult> CheckActionAsync(
        AgentAction action, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var result = new SafetyCheckResult { IsAllowed = true };
        
        // Check action type
        if (!_approvedActions.Contains(action.ActionType))
        {
            result.Warnings.Add($"Unknown action type: {action.ActionType}");
        }
        
        // Check tool
        if (action.ToolName == "sql_query")
        {
            var sqlCheck = await CheckSQLAsync(
                action.Parameters.GetValueOrDefault("query")?.ToString() ?? "", 
                session, 
                cancellationToken);
            
            if (!sqlCheck.IsAllowed)
            {
                return sqlCheck;
            }
            
            result.Warnings.AddRange(sqlCheck.Warnings);
        }
        
        // Check for sensitive data exposure
        foreach (var param in action.Parameters.Values)
        {
            var value = param?.ToString()?.ToLower() ?? "";
            foreach (var keyword in _sensitiveKeywords)
            {
                if (value.Contains(keyword))
                {
                    result.Warnings.Add($"Sensitive data detected in parameters: {keyword}");
                    result.RiskScore += 0.2;
                }
            }
        }
        
        // Check for suspicious patterns
        if (action.Parameters.Count == 0)
        {
            result.Warnings.Add("Action has no parameters");
        }
        
        _logger.LogDebug("Safety check for action {Action}: Allowed={Allowed}, Risk={Risk}", 
            action.ActionType, result.IsAllowed, result.RiskScore);
        
        return result;
    }
    
    /// <summary>
    /// Check prompt for injection attacks
    /// </summary>
    public async Task<SafetyCheckResult> CheckPromptAsync(
        string prompt, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var result = new SafetyCheckResult { IsAllowed = true };
        
        // Basic injection checks
        var injectionPatterns = new[]
        {
            "ignore previous", "ignore instructions", "disregard",
            "system prompt", "you are now", "forget all",
            "</s>", "<|", "[INST]", "[/INST]"
        };
        
        var lowerPrompt = prompt.ToLower();
        foreach (var pattern in injectionPatterns)
        {
            if (lowerPrompt.Contains(pattern))
            {
                result.Warnings.Add($"Potential prompt injection detected: {pattern}");
                result.RiskScore += 0.3;
            }
        }
        
        // Check for code blocks that might be malicious
        if (prompt.Contains("```") && (lowerPrompt.Contains("sudo") || lowerPrompt.Contains("rm -rf")))
        {
            result.Warnings.Add("Potentially dangerous command detected");
            result.RiskScore += 0.5;
        }
        
        // Use LLM for advanced injection detection
        if (result.RiskScore > 0)
        {
            _logger.LogWarning("Prompt injection detected: {Warnings}", 
                string.Join(", ", result.Warnings));
        }
        
        return result;
    }
    
    /// <summary>
    /// Check SQL for safety
    /// </summary>
    public Task<SafetyCheckResult> CheckSQLAsync(
        string sql, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var result = new SafetyCheckResult { IsAllowed = true };
        
        if (string.IsNullOrWhiteSpace(sql))
        {
            result.IsAllowed = false;
            result.Reason = "Empty SQL query";
            return Task.FromResult(result);
        }
        
        var upperSql = sql.ToUpperInvariant();
        
        // Check for dangerous operations
        foreach (var pattern in _dangerousSQLPatterns)
        {
            if (upperSql.Contains(pattern))
            {
                // Allow SELECT (should be the only one allowed)
                if (pattern.Trim() == "SELECT" && upperSql.StartsWith("SELECT"))
                {
                    continue;
                }
                
                result.IsAllowed = false;
                result.Reason = $"Forbidden SQL operation: {pattern.Trim()}";
                result.RiskScore = 1.0;
                
                _logger.LogWarning("SQL safety check failed: {Reason}", result.Reason);
                return Task.FromResult(result);
            }
        }
        
        // Check for dangerous characters
        if (sql.Contains("';") || sql.Contains("';") || sql.Contains("1=1"))
        {
            result.IsAllowed = false;
            result.Reason = "SQL injection pattern detected";
            result.RiskScore = 1.0;
            return Task.FromResult(result);
        }
        
        // Warn about complex queries
        if (sql.Length > 1000)
        {
            result.Warnings.Add("Very long SQL query - consider reviewing");
            result.RiskScore += 0.1;
        }
        
        return Task.FromResult(result);
    }
}

public class SafetyGuardConfig
{
    public bool EnablePromptInjectionDetection { get; set; } = true;
    public bool EnableSQLValidation { get; set; } = true;
    public bool EnableContentFiltering { get; set; } = true;
    public double MaxRiskScore { get; set; } = 0.8;
    public List<string> BlockedPatterns { get; set; } = new();
}
