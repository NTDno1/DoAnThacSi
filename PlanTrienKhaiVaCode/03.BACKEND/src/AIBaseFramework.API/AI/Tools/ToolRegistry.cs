// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Tool Registry & Plugin Architecture
// ============================================================

using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Agent;

namespace AIBaseFramework.AI.Tools;

// ============================================================
// TOOL DEFINITIONS
// ============================================================

/// <summary>
/// Base interface for all AI tools
/// </summary>
public interface IAITool
{
    /// <summary>
    /// Tool identifier
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Human-readable description
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Tool category
    /// </summary>
    ToolCategory Category { get; }
    
    /// <summary>
    /// Parameters schema (JSON Schema)
    /// </summary>
    string ParametersSchema { get; }
    
    /// <summary>
    /// Whether this tool requires approval before execution
    /// </summary>
    bool RequiresApproval { get; }
    
    /// <summary>
    /// Execute the tool
    /// </summary>
    Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Tool category
/// </summary>
public enum ToolCategory
{
    Search,
    Document,
    Database,
    API,
    Workflow,
    File,
    System,
    Custom
}

/// <summary>
/// Tool metadata
/// </summary>
public class ToolMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ToolCategory Category { get; set; }
    public string Version { get; set; } = "1.0.0";
    public string? Author { get; set; }
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, string> Parameters { get; set; } = new();
    public bool IsEnabled { get; set; } = true;
    public bool RequiresApproval { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

// ============================================================
// TOOL REGISTRY
// ============================================================

/// <summary>
/// Tool Registry - Manages all available AI tools
/// </summary>
public interface IToolRegistry
{
    /// <summary>
    /// Register a tool
    /// </summary>
    void RegisterTool(IAITool tool);
    
    /// <summary>
    /// Register multiple tools
    /// </summary>
    void RegisterTools(IEnumerable<IAITool> tools);
    
    /// <summary>
    /// Get a tool by name
    /// </summary>
    IAITool? GetTool(string name);
    
    /// <summary>
    /// Get all registered tools
    /// </summary>
    IEnumerable<IAITool> GetAllTools();
    
    /// <summary>
    /// Get tools by category
    /// </summary>
    IEnumerable<IAITool> GetToolsByCategory(ToolCategory category);
    
    /// <summary>
    /// Get tools metadata
    /// </summary>
    IEnumerable<ToolMetadata> GetToolsMetadata();
    
    /// <summary>
    /// Unregister a tool
    /// </summary>
    bool UnregisterTool(string name);
    
    /// <summary>
    /// Enable/disable a tool
    /// </summary>
    void SetToolEnabled(string name, bool enabled);
    
    /// <summary>
    /// Check if tool exists
    /// </summary>
    bool HasTool(string name);
}

/// <summary>
/// Tool Registry implementation
/// </summary>
public class ToolRegistry : IToolRegistry
{
    private readonly Dictionary<string, IAITool> _tools = new();
    private readonly ILogger<ToolRegistry> _logger;
    private readonly object _lock = new();
    
    public ToolRegistry(ILogger<ToolRegistry> logger)
    {
        _logger = logger;
    }
    
    public void RegisterTool(IAITool tool)
    {
        lock (_lock)
        {
            if (_tools.ContainsKey(tool.Name))
            {
                _logger.LogWarning("Tool already registered: {Name}", tool.Name);
                return;
            }
            
            _tools[tool.Name] = tool;
            _logger.LogInformation("Registered tool: {Name} ({Category})", tool.Name, tool.Category);
        }
    }
    
    public void RegisterTools(IEnumerable<IAITool> tools)
    {
        foreach (var tool in tools)
        {
            RegisterTool(tool);
        }
    }
    
    public IAITool? GetTool(string name)
    {
        lock (_lock)
        {
            return _tools.TryGetValue(name, out var tool) ? tool : null;
        }
    }
    
    public IEnumerable<IAITool> GetAllTools()
    {
        lock (_lock)
        {
            return _tools.Values.ToList();
        }
    }
    
    public IEnumerable<IAITool> GetToolsByCategory(ToolCategory category)
    {
        lock (_lock)
        {
            return _tools.Values.Where(t => t.Category == category).ToList();
        }
    }
    
    public IEnumerable<ToolMetadata> GetToolsMetadata()
    {
        lock (_lock)
        {
            return _tools.Values.Select(t => new ToolMetadata
            {
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                RequiresApproval = t.RequiresApproval
            }).ToList();
        }
    }
    
    public bool UnregisterTool(string name)
    {
        lock (_lock)
        {
            var removed = _tools.Remove(name);
            if (removed)
            {
                _logger.LogInformation("Unregistered tool: {Name}", name);
            }
            return removed;
        }
    }
    
    public void SetToolEnabled(string name, bool enabled)
    {
        // In a full implementation, this would toggle enabled state
        _logger.LogInformation("Tool {Name} enabled: {Enabled}", name, enabled);
    }
    
    public bool HasTool(string name)
    {
        lock (_lock)
        {
            return _tools.ContainsKey(name);
        }
    }
}

// ============================================================
// BUILT-IN TOOLS
// ============================================================

/// <summary>
/// Search tool
/// </summary>
public class SearchTool : IAITool
{
    private readonly AIBaseFramework.AI.Search.ISearchEngine _searchEngine;
    
    public string Name => "search";
    public string Description => "Search documents and knowledge base";
    public ToolCategory Category => ToolCategory.Search;
    public string ParametersSchema => @"{
        ""type"": ""object"",
        ""properties"": {
            ""query"": { ""type"": ""string"", ""description"": ""Search query"" },
            ""limit"": { ""type"": ""integer"", ""default"": 10 },
            ""searchType"": { ""type"": ""string"", ""enum"": [""semantic"", ""keyword"", ""hybrid""] }
        },
        ""required"": [""query""]
    }";
    public bool RequiresApproval => false;
    
    public SearchTool(AIBaseFramework.AI.Search.ISearchEngine searchEngine)
    {
        _searchEngine = searchEngine;
    }
    
    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var query = parameters.GetValueOrDefault("query")?.ToString() ?? "";
        var limit = int.TryParse(parameters.GetValueOrDefault("limit")?.ToString(), out var l) ? l : 10;
        
        var request = new AIBaseFramework.AI.Search.SearchRequest
        {
            Query = query,
            Limit = limit,
            UserId = session.UserId,
            TenantId = session.TenantId
        };
        
        var response = await _searchEngine.SearchAsync(request, cancellationToken);
        
        var results = response.Results.Select((r, i) => 
            $"{i + 1}. {r.Title}\n   {r.Content.Substring(0, Math.Min(200, r.Content.Length))}...")
            .ToList();
        
        return $"Found {response.TotalResults} results:\n\n" + string.Join("\n\n", results);
    }
}

/// <summary>
/// RAG Query tool
/// </summary>
public class RAGQueryTool : IAITool
{
    private readonly AIBaseFramework.AI.RAG.IRAGPipeline _ragPipeline;
    
    public string Name => "rag_query";
    public string Description => "Answer questions based on document context";
    public ToolCategory Category => ToolCategory.Search;
    public string ParametersSchema => @"{
        ""type"": ""object"",
        ""properties"": {
            ""query"": { ""type"": ""string"", ""description"": ""User question"" },
            ""maxChunks"": { ""type"": ""integer"", ""default"": 5 }
        },
        ""required"": [""query""]
    }";
    public bool RequiresApproval => false;
    
    public RAGQueryTool(AIBaseFramework.AI.RAG.IRAGPipeline ragPipeline)
    {
        _ragPipeline = ragPipeline;
    }
    
    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var query = parameters.GetValueOrDefault("query")?.ToString() ?? "";
        var maxChunks = int.TryParse(parameters.GetValueOrDefault("maxChunks")?.ToString(), out var m) ? m : 5;
        
        var request = new AIBaseFramework.AI.RAG.RAGRequest
        {
            Query = query,
            MaxChunks = maxChunks,
            UserId = session.UserId,
            TenantId = session.TenantId
        };
        
        var response = await _ragPipeline.QueryAsync(request, cancellationToken);
        
        return response.Answer;
    }
}

/// <summary>
/// SQL Query tool
/// </summary>
public class SQLQueryTool : IAITool
{
    private readonly AIBaseFramework.AI.SQL.ISQLEngine _sqlEngine;
    
    public string Name => "sql_query";
    public string Description => "Execute SQL queries on the database";
    public ToolCategory Category => ToolCategory.Database;
    public string ParametersSchema => @"{
        ""type"": ""object"",
        ""properties"": {
            ""query"": { ""type"": ""string"", ""description"": ""Natural language query"" }
        },
        ""required"": [""query""]
    }";
    public bool RequiresApproval => true;
    
    public SQLQueryTool(AIBaseFramework.AI.SQL.ISQLEngine sqlEngine)
    {
        _sqlEngine = sqlEngine;
    }
    
    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var query = parameters.GetValueOrDefault("query")?.ToString() ?? "";
        
        var request = new AIBaseFramework.AI.SQL.SQLQueryRequest
        {
            NaturalLanguageQuery = query,
            UserId = session.UserId,
            TenantId = session.TenantId
        };
        
        var response = await _sqlEngine.QueryAsync(request, cancellationToken);
        
        if (!response.Success)
        {
            return $"Query failed: {response.Error}";
        }
        
        return $"Query executed successfully. {response.RowCount} rows affected.\n\n{response.FormattedResults}";
    }
}

/// <summary>
/// API Caller tool
/// </summary>
public class APICallerTool : IAITool
{
    private readonly IAPIConnectorRegistry _connectorRegistry;
    private readonly IAgentSafetyGuard _safetyGuard;
    
    public string Name => "api_caller";
    public string Description => "Call external APIs";
    public ToolCategory Category => ToolCategory.API;
    public string ParametersSchema => @"{
        ""type"": ""object"",
        ""properties"": {
            ""endpoint"": { ""type"": ""string"", ""description"": ""API endpoint"" },
            ""method"": { ""type"": ""string"", ""enum"": [""GET"", ""POST"", ""PUT"", ""DELETE""] },
            ""data"": { ""type"": ""object"", ""description"": ""Request payload"" }
        },
        ""required"": [""endpoint"", ""method""]
    }";
    public bool RequiresApproval => true;
    
    public APICallerTool(IAPIConnectorRegistry connectorRegistry, IAgentSafetyGuard safetyGuard)
    {
        _connectorRegistry = connectorRegistry;
        _safetyGuard = safetyGuard;
    }
    
    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var endpoint = parameters.GetValueOrDefault("endpoint")?.ToString() ?? "";
        var method = parameters.GetValueOrDefault("method")?.ToString() ?? "GET";
        
        // Get connector for endpoint
        var connector = _connectorRegistry.GetConnector(endpoint);
        
        if (connector == null)
        {
            return $"No connector found for endpoint: {endpoint}";
        }
        
        // Execute via connector
        var result = await connector.CallAsync(method, endpoint, parameters, session, cancellationToken);
        
        return System.Text.Json.JsonSerializer.Serialize(result);
    }
}

/// <summary>
/// Context Retriever tool
/// </summary>
public class ContextRetrieverTool : IAITool
{
    private readonly IAI.Gateway.IAI Gateway _aiGateway;
    private readonly IAgentMemoryService _memoryService;
    
    public string Name => "context_retriever";
    public string Description => "Retrieve relevant context from conversation history";
    public ToolCategory Category => ToolCategory.Custom;
    public string ParametersSchema => @"{
        ""type"": ""object"",
        ""properties"": {
            ""query"": { ""type"": ""string"" },
            ""limit"": { ""type"": ""integer"", ""default"": 5 }
        },
        ""required"": [""query""]
    }";
    public bool RequiresApproval => false;
    
    public ContextRetrieverTool(IAI.Gateway.IAI Gateway aiGateway, IAgentMemoryService memoryService)
    {
        _aiGateway = aiGateway;
        _memoryService = memoryService;
    }
    
    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var query = parameters.GetValueOrDefault("query")?.ToString() ?? "";
        var limit = int.TryParse(parameters.GetValueOrDefault("limit")?.ToString(), out var l) ? l : 5;
        
        // Get relevant memories
        var memories = await _memoryService.GetRelevantMemoriesAsync(
            session.SessionId, 
            query, 
            limit, 
            cancellationToken);
        
        if (!memories.Any())
        {
            return "No relevant context found in conversation history.";
        }
        
        return string.Join("\n\n", memories.Select(m => $"[{m.Type}]: {m.Content}"));
    }
}

/// <summary>
/// LLM Response tool
/// </summary>
public class LLMResponseTool : IAITool
{
    private readonly IAI.Gateway.IAI Gateway _aiGateway;
    
    public string Name => "llm";
    public string Description => "Generate text response using LLM";
    public ToolCategory Category => ToolCategory.Custom;
    public string ParametersSchema => @"{
        ""type"": ""object"",
        ""properties"": {
            ""prompt"": { ""type"": ""string"" },
            ""use_context"": { ""type"": ""boolean"", ""default"": true }
        },
        ""required"": [""prompt""]
    }";
    public bool RequiresApproval => false;
    
    public LLMResponseTool(IAI.Gateway.IAI Gateway aiGateway)
    {
        _aiGateway = aiGateway;
    }
    
    public async Task<string> ExecuteAsync(
        Dictionary<string, object> parameters, 
        AgentSession session, 
        CancellationToken cancellationToken = default)
    {
        var prompt = parameters.GetValueOrDefault("prompt")?.ToString() ?? "";
        var useContext = bool.TryParse(parameters.GetValueOrDefault("use_context")?.ToString(), out var uc) && uc;
        
        var messages = new List<AIBaseFramework.AI.Providers.Abstractions.ChatMessage>();
        
        if (useContext)
        {
            // Add recent messages as context
            foreach (var msg in session.Messages.TakeLast(5))
            {
                messages.Add(new AIBaseFramework.AI.Providers.Abstractions.ChatMessage
                {
                    Role = (AIBaseFramework.AI.Providers.Abstractions.MessageRole)msg.Role,
                    Content = msg.Content
                });
            }
        }
        
        messages.Add(new AIBaseFramework.AI.Providers.Abstractions.ChatMessage
        {
            Role = AIBaseFramework.AI.Providers.Abstractions.MessageRole.User,
            Content = prompt
        });
        
        var response = await _aiGateway.ChatAsync(new AIBaseFramework.AI.Providers.Abstractions.ChatRequest
        {
            Messages = messages,
            Options = new AIBaseFramework.AI.Providers.Abstractions.AIRequestOptions
            {
                Temperature = 0.7,
                MaxTokens = 1000,
                UserId = session.UserId,
                TenantId = session.TenantId
            }
        }, cancellationToken);
        
        return response.Content;
    }
}

// ============================================================
// API CONNECTOR REGISTRY
// ============================================================

public interface IAPIConnector
{
    string Name { get; }
    string BaseUrl { get; }
    bool CanHandle(string endpoint);
    Task<object> CallAsync(string method, string endpoint, Dictionary<string, object> parameters, AgentSession session, CancellationToken cancellationToken);
}

public interface IAPIConnectorRegistry
{
    void RegisterConnector(IAPIConnector connector);
    IAPIConnector? GetConnector(string endpoint);
    IEnumerable<IAPIConnector> GetAllConnectors();
}

public class APIConnectorRegistry : IAPIConnectorRegistry
{
    private readonly List<IAPIConnector> _connectors = new();
    
    public void RegisterConnector(IAPIConnector connector)
    {
        _connectors.Add(connector);
    }
    
    public IAPIConnector? GetConnector(string endpoint)
    {
        return _connectors.FirstOrDefault(c => c.CanHandle(endpoint));
    }
    
    public IEnumerable<IAPIConnector> GetAllConnectors()
    {
        return _connectors.ToList();
    }
}

// ============================================================
// AGENT MEMORY SERVICE
// ============================================================

public class MemoryItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SessionId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float[]? Embedding { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public interface IAgentMemoryService
{
    Task AddMemoryAsync(MemoryItem memory, CancellationToken cancellationToken = default);
    Task<List<MemoryItem>> GetRelevantMemoriesAsync(string sessionId, string query, int limit, CancellationToken cancellationToken = default);
    Task<List<MemoryItem>> GetRecentMemoriesAsync(string sessionId, int limit, CancellationToken cancellationToken = default);
    Task ClearSessionMemoriesAsync(string sessionId, CancellationToken cancellationToken = default);
}
