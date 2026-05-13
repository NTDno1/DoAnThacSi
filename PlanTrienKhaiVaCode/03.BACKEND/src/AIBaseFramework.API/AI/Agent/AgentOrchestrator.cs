// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: AI Agent Framework - Intent Classification & Task Planning
// ============================================================

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Gateway;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Agent;

// ============================================================
// AGENT TYPES
// ============================================================

/// <summary>
/// Agent session state
/// </summary>
public class AgentSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public List<AgentMessage> Messages { get; set; } = new();
    public AgentContext Context { get; set; } = new();
    public Dictionary<string, object> Memory { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Agent message
/// </summary>
public class AgentMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IntentClassification? ClassifiedIntent { get; set; }
    public List<AgentAction>? Actions { get; set; }
    public AgentAction? ExecutingAction { get; set; }
}

/// <summary>
/// Agent context for memory and state
/// </summary>
public class AgentContext
{
    public string? CurrentTask { get; set; }
    public List<string> CompletedTasks { get; set; } = new();
    public List<string> FailedTasks { get; set; } = new();
    public Dictionary<string, object> Variables { get; set; } = new();
    public Dictionary<string, object> UserPreferences { get; set; } = new();
    public string? LastToolUsed { get; set; }
    public int StepCount { get; set; }
}

/// <summary>
/// Intent classification result
/// </summary>
public class IntentClassification
{
    public string Intent { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public Dictionary<string, object> Entities { get; set; } = new();
    public string? SuggestedAction { get; set; }
    public List<string> RequiredTools { get; set; } = new();
    public Dictionary<string, string> Parameters { get; set; } = new();
}

/// <summary>
/// Agent action
/// </summary>
public class AgentAction
{
    public string ActionId { get; set; } = Guid.NewGuid().ToString();
    public string ActionType { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
    public AgentActionStatus Status { get; set; } = AgentActionStatus.Pending;
    public string? Result { get; set; }
    public string? Error { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue 
        ? CompletedAt.Value - StartedAt.Value 
        : null;
}

/// <summary>
/// Action status
/// </summary>
public enum AgentActionStatus
{
    Pending,
    InProgress,
    AwaitingApproval,
    Completed,
    Failed,
    Cancelled,
    Skipped
}

/// <summary>
/// Agent request
/// </summary>
public class AgentRequest
{
    public string Input { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public AgentMode Mode { get; set; } = AgentMode.Auto;
    public int MaxSteps { get; set; } = 10;
    public int MaxIterations { get; set; } = 5;
    public bool StreamResponse { get; set; } = true;
    public Dictionary<string, string>? Context { get; set; }
}

/// <summary>
/// Agent mode
/// </summary>
public enum AgentMode
{
    /// <summary>
    /// Fully autonomous execution
    /// </summary>
    Auto,
    
    /// <summary>
    /// Confirm each action before execution
    /// </summary>
    ConfirmEachStep,
    
    /// <summary>
    /// Only plan, don't execute
    /// </summary>
    PlanOnly,
    
    /// <summary>
    /// Step-by-step with manual confirmation for sensitive actions
    /// </summary>
    SemiAuto
}

/// <summary>
/// Agent response
/// </summary>
public class AgentResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public IntentClassification? Intent { get; set; }
    public List<AgentAction> Actions { get; set; } = new();
    public List<AgentAction> PendingApprovals { get; set; } = new();
    public bool IsComplete { get; set; }
    public bool RequiresUserInput { get; set; }
    public string? UserInputPrompt { get; set; }
    public AgentMetrics Metrics { get; set; } = new();
}

/// <summary>
/// Agent metrics
/// </summary>
public class AgentMetrics
{
    public int TotalSteps { get; set; }
    public int CompletedActions { get; set; }
    public int FailedActions { get; set; }
    public int PendingApprovals { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan PlanningTime { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public int LLMCalls { get; set; }
    public int ToolCalls { get; set; }
    public double TotalCost { get; set; }
}

// ============================================================
// AGENT ORCHESTRATOR
// ============================================================

/// <summary>
/// AI Agent Orchestrator interface
/// </summary>
public interface IAgentOrchestrator
{
    Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> StreamProcessAsync(AgentRequest request, CancellationToken cancellationToken = default);
    Task<AgentSession?> GetSessionAsync(string sessionId);
    Task<AgentSession> CreateSessionAsync(string? userId, string? tenantId);
    Task<AgentAction> ApproveActionAsync(string sessionId, string actionId, bool approved, Dictionary<string, object>? modifiedParameters = null);
    Task CancelSessionAsync(string sessionId);
}

/// <summary>
/// AI Agent Orchestrator implementation
/// </summary>
public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly IAI Gateway _aiGateway;
    private readonly IToolRegistry _toolRegistry;
    private readonly IAgentSafetyGuard _safetyGuard;
    private readonly ISessionStore _sessionStore;
    private readonly ILogger<AgentOrchestrator> _logger;
    private readonly AgentConfig _config;
    
    // Intent patterns for classification
    private static readonly Dictionary<string, IntentDefinition> _intentDefinitions = new()
    {
        ["search"] = new IntentDefinition
        {
            Intent = "search",
            Patterns = new[] { "tìm", "search", "tra cứu", "tìm kiếm", "find", "lookup" },
            RequiredTools = new[] { "search", "document_retriever" },
            RequiresApproval = false
        },
        ["chat"] = new IntentDefinition
        {
            Intent = "chat",
            Patterns = new[] { "trò chuyện", "chat", "nói chuyện", "hỏi", "ask", "question" },
            RequiredTools = Array.Empty<string>(),
            RequiresApproval = false
        },
        ["create"] = new IntentDefinition
        {
            Intent = "create",
            Patterns = new[] { "tạo", "create", "mới", "new", "thêm", "add" },
            RequiredTools = new[] { "document_creator", "api_caller" },
            RequiresApproval = true
        },
        ["update"] = new IntentDefinition
        {
            Intent = "update",
            Patterns = new[] { "cập nhật", "update", "sửa", "edit", "modify", "change" },
            RequiredTools = new[] { "api_caller" },
            RequiresApproval = true
        },
        ["delete"] = new IntentDefinition
        {
            Intent = "delete",
            Patterns = new[] { "xóa", "delete", "remove", "bỏ" },
            RequiredTools = new[] { "api_caller" },
            RequiresApproval = true
        },
        ["approve"] = new IntentDefinition
        {
            Intent = "approve",
            Patterns = new[] { "duyệt", "approve", "confirm", "chấp nhận", "accept" },
            RequiredTools = new[] { "approval_handler" },
            RequiresApproval = false
        },
        ["workflow"] = new IntentDefinition
        {
            Intent = "workflow",
            Patterns = new[] { "workflow", "quy trình", "process", "pipeline" },
            RequiredTools = new[] { "workflow_engine" },
            RequiresApproval = false
        },
        ["sql_query"] = new IntentDefinition
        {
            Intent = "sql_query",
            Patterns = new[] { "truy vấn", "query", "database", "sql", "cơ sở dữ liệu" },
            RequiredTools = new[] { "sql_engine" },
            RequiresApproval = false
        }
    };
    
    public AgentOrchestrator(
        IAI Gateway aiGateway,
        IToolRegistry toolRegistry,
        IAgentSafetyGuard safetyGuard,
        ISessionStore sessionStore,
        ILogger<AgentOrchestrator> logger,
        AgentConfig config)
    {
        _aiGateway = aiGateway;
        _toolRegistry = toolRegistry;
        _safetyGuard = safetyGuard;
        _sessionStore = sessionStore;
        _logger = logger;
        _config = config;
    }
    
    /// <summary>
    /// Process user input and execute agentic actions
    /// </summary>
    public async Task<AgentResponse> ProcessAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var metrics = new AgentMetrics();
        
        _logger.LogInformation("Processing agent request: {Input}", request.Input.Substring(0, Math.Min(100, request.Input.Length)));
        
        try
        {
            // Step 1: Get or create session
            var session = string.IsNullOrEmpty(request.SessionId)
                ? await CreateSessionAsync(request.UserId, request.TenantId)
                : await _sessionStore.GetAsync(request.SessionId) 
                    ?? await CreateSessionAsync(request.UserId, request.TenantId);
            
            if (session == null)
            {
                throw new InvalidOperationException("Failed to create or retrieve session");
            }
            
            // Step 2: Add user message
            session.Messages.Add(new AgentMessage
            {
                Role = MessageRole.User,
                Content = request.Input
            });
            
            // Step 3: Classify intent
            var planningStopwatch = Stopwatch.StartNew();
            var intent = await ClassifyIntentAsync(request.Input, session, cancellationToken);
            metrics.PlanningTime = planningStopwatch.Elapsed;
            
            _logger.LogInformation("Classified intent: {Intent} (confidence: {Confidence:P2})", 
                intent.Intent, intent.Confidence);
            
            // Step 4: Plan actions
            var actions = await PlanActionsAsync(intent, session, cancellationToken);
            
            // Step 5: Execute actions
            var executionStopwatch = Stopwatch.StartNew();
            var executedActions = new List<AgentAction>();
            var pendingApprovals = new List<AgentAction>();
            
            for (int i = 0; i < actions.Count && i < request.MaxSteps; i++)
            {
                var action = actions[i];
                action.Status = AgentActionStatus.InProgress;
                action.StartedAt = DateTime.UtcNow;
                
                // Check safety
                var safetyResult = await _safetyGuard.CheckActionAsync(action, session, cancellationToken);
                if (!safetyResult.IsAllowed)
                {
                    _logger.LogWarning("Action blocked by safety guard: {Reason}", safetyResult.Reason);
                    action.Status = AgentActionStatus.Failed;
                    action.Error = safetyResult.Reason;
                    executedActions.Add(action);
                    continue;
                }
                
                // Check if approval required
                if (intent.RequiresApproval || _config.ApprovalRequiredIntents.Contains(intent.Intent))
                {
                    action.Status = AgentActionStatus.AwaitingApproval;
                    pendingApprovals.Add(action);
                    session.Messages.Add(new AgentMessage
                    {
                        Role = MessageRole.Assistant,
                        Content = $"I need your approval to: {action.ActionType}",
                        ExecutingAction = action
                    });
                    continue;
                }
                
                // Execute action
                var result = await ExecuteActionAsync(action, session, cancellationToken);
                executedActions.Add(action);
                metrics.ToolCalls++;
                
                if (result.Error != null)
                {
                    _logger.LogWarning("Action failed: {Action} - {Error}", action.ToolName, result.Error);
                }
            }
            
            metrics.ExecutionTime = executionStopwatch.Elapsed;
            metrics.CompletedActions = executedActions.Count(a => a.Status == AgentActionStatus.Completed);
            metrics.FailedActions = executedActions.Count(a => a.Status == AgentActionStatus.Failed);
            metrics.PendingApprovals = pendingApprovals.Count;
            metrics.TotalSteps = executedActions.Count;
            
            // Step 6: Generate final response
            var response = await GenerateResponseAsync(intent, executedActions, pendingApprovals, session, cancellationToken);
            
            // Add assistant message
            session.Messages.Add(new AgentMessage
            {
                Role = MessageRole.Assistant,
                Content = response.Output,
                ClassifiedIntent = intent,
                Actions = executedActions
            });
            
            session.LastActivityAt = DateTime.UtcNow;
            await _sessionStore.SaveAsync(session);
            
            stopwatch.Stop();
            metrics.TotalDuration = stopwatch.Elapsed;
            metrics.LLMCalls = 1;
            
            response.SessionId = session.SessionId;
            response.Intent = intent;
            response.Actions = executedActions;
            response.PendingApprovals = pendingApprovals;
            response.IsComplete = pendingApprovals.Count == 0;
            response.RequiresUserInput = pendingApprovals.Count > 0;
            response.UserInputPrompt = pendingApprovals.Count > 0 
                ? $"Please confirm: {pendingApprovals[0].ActionType}" 
                : null;
            response.Metrics = metrics;
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent processing failed");
            throw;
        }
    }
    
    /// <summary>
    /// Stream agent response for real-time output
    /// </summary>
    public async IAsyncEnumerable<string> StreamProcessAsync(
        AgentRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await ProcessAsync(request, cancellationToken);
        
        yield return $"Intent: {response.Intent?.Intent}\n";
        
        foreach (var action in response.Actions)
        {
            yield return $"[Action] {action.ToolName}: {action.Status}\n";
            if (action.Result != null)
            {
                yield return $"  Result: {action.Result.Substring(0, Math.Min(200, action.Result.Length))}...\n";
            }
        }
        
        yield return $"\n{response.Output}";
    }
    
    public async Task<AgentSession?> GetSessionAsync(string sessionId)
    {
        return await _sessionStore.GetAsync(sessionId);
    }
    
    public async Task<AgentSession> CreateSessionAsync(string? userId, string? tenantId)
    {
        var session = new AgentSession
        {
            UserId = userId,
            TenantId = tenantId
        };
        
        await _sessionStore.SaveAsync(session);
        return session;
    }
    
    public async Task<AgentAction> ApproveActionAsync(
        string sessionId, 
        string actionId, 
        bool approved, 
        Dictionary<string, object>? modifiedParameters = null)
    {
        var session = await _sessionStore.GetAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session not found: {sessionId}");
        }
        
        var action = session.Messages
            .SelectMany(m => m.Actions ?? Enumerable.Empty<AgentAction>())
            .FirstOrDefault(a => a.ActionId == actionId);
        
        if (action == null)
        {
            throw new InvalidOperationException($"Action not found: {actionId}");
        }
        
        if (modifiedParameters != null)
        {
            action.Parameters = modifiedParameters;
        }
        
        if (approved)
        {
            return await ExecuteActionAsync(action, session, CancellationToken.None);
        }
        else
        {
            action.Status = AgentActionStatus.Cancelled;
            await _sessionStore.SaveAsync(session);
            return action;
        }
    }
    
    public async Task CancelSessionAsync(string sessionId)
    {
        var session = await _sessionStore.GetAsync(sessionId);
        if (session != null)
        {
            session.Context.Variables["cancelled"] = true;
            await _sessionStore.SaveAsync(session);
        }
    }
    
    // ============================================================
    // INTENT CLASSIFICATION
    // ============================================================
    
    private async Task<IntentClassification> ClassifyIntentAsync(
        string input, 
        AgentSession session, 
        CancellationToken cancellationToken)
    {
        // Quick pattern matching first
        var quickMatch = QuickIntentMatch(input);
        if (quickMatch != null && quickMatch.Confidence > 0.8)
        {
            return quickMatch;
        }
        
        // Use LLM for complex classification
        var contextPrompt = session.Messages.Count > 0
            ? $"\n\nPrevious conversation:\n{string.Join("\n", session.Messages.TakeLast(3).Select(m => $"{m.Role}: {m.Content}"))}"
            : "";
        
        var systemPrompt = @"You are an intent classifier. Analyze the user input and classify it into one of these intents:
- search: Looking for information, documents, or data
- chat: Casual conversation or questions
- create: Creating new records, documents, or entities
- update: Modifying existing records
- delete: Removing records
- approve: Confirming or approving an action
- workflow: Managing workflows or processes
- sql_query: Database queries

Return JSON:
{
  ""intent"": ""intent_name"",
  ""confidence"": 0.0-1.0,
  ""entities"": { ""entity_name"": ""entity_value"" },
  ""parameters"": { ""param_name"": ""param_value"" }
}";

        var chatResponse = await _aiGateway.ChatAsync(new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = systemPrompt },
                new() { Role = MessageRole.User, Content = $"Input: {input}{contextPrompt}" }
            },
            Options = new AIRequestOptions { Temperature = 0.3, MaxTokens = 500 }
        }, cancellationToken);
        
        try
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<IntentClassification>(chatResponse.Content);
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse intent classification response");
        }
        
        // Fallback
        return quickMatch ?? new IntentClassification
        {
            Intent = "chat",
            Confidence = 0.5
        };
    }
    
    private IntentClassification? QuickIntentMatch(string input)
    {
        var lowerInput = input.ToLower();
        
        foreach (var (intent, definition) in _intentDefinitions)
        {
            foreach (var pattern in definition.Patterns)
            {
                if (lowerInput.Contains(pattern.ToLower()))
                {
                    return new IntentClassification
                    {
                        Intent = intent,
                        Confidence = 0.9,
                        RequiredTools = definition.RequiredTools.ToList(),
                        Entities = ExtractEntities(input)
                    };
                }
            }
        }
        
        return null;
    }
    
    private Dictionary<string, object> ExtractEntities(string input)
    {
        var entities = new Dictionary<string, object>();
        
        // Simple entity extraction (in production, use NER)
        var actionWords = new[] { "tạo", "create", "cập nhật", "update", "xóa", "delete", "tìm", "search" };
        foreach (var word in actionWords)
        {
            if (input.ToLower().Contains(word))
            {
                entities["action_verb"] = word;
                break;
            }
        }
        
        // Extract potential entity names (capitalized words or quoted strings)
        var quotedMatches = System.Text.RegularExpressions.Regex.Matches(input, @"""([^""]+)""");
        if (quotedMatches.Count > 0)
        {
            entities["target"] = quotedMatches[0].Groups[1].Value;
        }
        
        return entities;
    }
    
    // ============================================================
    // ACTION PLANNING
    // ============================================================
    
    private async Task<List<AgentAction>> PlanActionsAsync(
        IntentClassification intent, 
        AgentSession session, 
        CancellationToken cancellationToken)
    {
        var actions = new List<AgentAction>();
        
        // Map intent to actions
        switch (intent.Intent)
        {
            case "search":
                actions.Add(new AgentAction
                {
                    ActionType = "SearchDocuments",
                    ToolName = "search",
                    Parameters = new Dictionary<string, object>
                    {
                        ["query"] = intent.Parameters.GetValueOrDefault("query", session.Context.CurrentTask ?? intent.Intent),
                        ["limit"] = 10
                    }
                });
                
                if (intent.RequiredTools.Contains("document_retriever"))
                {
                    actions.Add(new AgentAction
                    {
                        ActionType = "RetrieveDocumentContents",
                        ToolName = "document_retriever",
                        Parameters = new Dictionary<string, object>()
                    });
                }
                break;
                
            case "chat":
                actions.Add(new AgentAction
                {
                    ActionType = "GenerateResponse",
                    ToolName = "llm",
                    Parameters = new Dictionary<string, object>
                    {
                        ["use_context"] = true
                    }
                });
                break;
                
            case "create":
            case "update":
            case "delete":
                actions.Add(new AgentAction
                {
                    ActionType = intent.Intent switch
                    {
                        "create" => "CreateEntity",
                        "update" => "UpdateEntity",
                        "delete" => "DeleteEntity",
                        _ => "Unknown"
                    },
                    ToolName = "api_caller",
                    Parameters = new Dictionary<string, object>
                    {
                        ["entity_type"] = intent.Entities.GetValueOrDefault("entity_type", "unknown"),
                        ["data"] = intent.Parameters
                    }
                });
                break;
                
            case "sql_query":
                actions.Add(new AgentAction
                {
                    ActionType = "ExecuteSQLQuery",
                    ToolName = "sql_engine",
                    Parameters = new Dictionary<string, object>
                    {
                        ["natural_language_query"] = intent.Parameters.GetValueOrDefault("query", session.Context.CurrentTask ?? "")
                    }
                });
                break;
                
            case "workflow":
                actions.Add(new AgentAction
                {
                    ActionType = "ExecuteWorkflow",
                    ToolName = "workflow_engine",
                    Parameters = intent.Parameters
                });
                break;
        }
        
        // Check if additional context retrieval is needed
        if (session.Context.StepCount == 0 && intent.Intent != "chat")
        {
            actions.Insert(0, new AgentAction
            {
                ActionType = "RetrieveContext",
                ToolName = "context_retriever",
                Parameters = new Dictionary<string, object>
                {
                    ["query"] = session.Context.CurrentTask ?? intent.Intent
                }
            });
        }
        
        return actions;
    }
    
    // ============================================================
    // ACTION EXECUTION
    // ============================================================
    
    private async Task<AgentAction> ExecuteActionAsync(
        AgentAction action, 
        AgentSession session, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing action: {Tool} ({Type})", action.ToolName, action.ActionType);
        
        try
        {
            var tool = _toolRegistry.GetTool(action.ToolName);
            
            if (tool == null)
            {
                // Fall back to LLM
                action.Result = await ExecuteWithLLMAsync(action, session, cancellationToken);
            }
            else
            {
                action.Result = await tool.ExecuteAsync(action.Parameters, session, cancellationToken);
            }
            
            action.Status = AgentActionStatus.Completed;
            action.CompletedAt = DateTime.UtcNow;
            session.Context.LastToolUsed = action.ToolName;
            session.Context.CompletedTasks.Add(action.ActionType);
            session.Context.StepCount++;
            
            _logger.LogInformation("Action completed: {Tool}", action.ToolName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Action execution failed: {Tool}", action.ToolName);
            action.Status = AgentActionStatus.Failed;
            action.Error = ex.Message;
            action.CompletedAt = DateTime.UtcNow;
            session.Context.FailedTasks.Add(action.ActionType);
        }
        
        return action;
    }
    
    private async Task<string> ExecuteWithLLMAsync(
        AgentAction action, 
        AgentSession session, 
        CancellationToken cancellationToken)
    {
        var context = string.Join("\n", session.Messages.TakeLast(5).Select(m => 
            $"{m.Role}: {m.Content}"));
        
        var systemPrompt = @"You are an AI assistant executing a task. Based on the context and action type, provide a helpful response.
If you need information, use your knowledge. If you cannot complete the task, explain why.";

        var response = await _aiGateway.ChatAsync(new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = systemPrompt },
                new() { Role = MessageRole.User, Content = $"Action: {action.ActionType}\nParameters: {System.Text.Json.JsonSerializer.Serialize(action.Parameters)}\n\nContext:\n{context}" }
            },
            Options = new AIRequestOptions { Temperature = 0.7, MaxTokens = 1000 }
        }, cancellationToken);
        
        return response.Content;
    }
    
    // ============================================================
    // RESPONSE GENERATION
    // ============================================================
    
    private async Task<string> GenerateResponseAsync(
        IntentClassification intent, 
        List<AgentAction> actions, 
        List<AgentAction> pendingApprovals,
        AgentSession session, 
        CancellationToken cancellationToken)
    {
        var completedResults = actions
            .Where(a => a.Status == AgentActionStatus.Completed && a.Result != null)
            .Select(a => a.Result!)
            .ToList();
        
        if (pendingApprovals.Count > 0)
        {
            return $"I need your approval to proceed with: {string.Join(", ", pendingApprovals.Select(a => a.ActionType))}\n\n" +
                   "Please confirm if you'd like me to continue.";
        }
        
        if (completedResults.Count == 0)
        {
            // Use LLM to generate natural response
            var response = await _aiGateway.ChatAsync(new ChatRequest
            {
                Messages = new List<ChatMessage>
                {
                    new() { Role = MessageRole.System, Content = "Summarize the following results in a helpful, concise manner:" },
                    new() { Role = MessageRole.User, Content = string.Join("\n\n", session.Messages.TakeLast(2).Select(m => m.Content)) }
                },
                Options = new AIRequestOptions { Temperature = 0.7, MaxTokens = 500 }
            }, cancellationToken);
            
            return response.Content;
        }
        
        return string.Join("\n\n", completedResults);
    }
}

/// <summary>
/// Intent definition
/// </summary>
public class IntentDefinition
{
    public string Intent { get; set; } = string.Empty;
    public string[] Patterns { get; set; } = Array.Empty<string>();
    public string[] RequiredTools { get; set; } = Array.Empty<string>();
    public bool RequiresApproval { get; set; }
}

// ============================================================
// SESSION STORE
// ============================================================

public interface ISessionStore
{
    Task<AgentSession?> GetAsync(string sessionId);
    Task SaveAsync(AgentSession session);
    Task DeleteAsync(string sessionId);
    Task<List<AgentSession>> GetUserSessionsAsync(string userId);
}

// ============================================================
// AGENT CONFIG
// ============================================================

public class AgentConfig
{
    public int MaxSteps { get; set; } = 10;
    public int MaxIterations { get; set; } = 5;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public List<string> ApprovalRequiredIntents { get; set; } = new() { "create", "update", "delete" };
    public bool EnableMemory { get; set; } = true;
    public int MaxMemoryItems { get; set; } = 100;
    public bool EnableReflection { get; set; } = true;
}
