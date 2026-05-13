// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: AI Provider Abstraction Layer
// Version: 1.0.0
// ============================================================

namespace AIBaseFramework.AI.Providers.Abstractions;

// ============================================================
// CORE ABSTRACTIONS
// ============================================================

/// <summary>
/// Common options for AI requests
/// </summary>
public class AIRequestOptions
{
    public string? ModelId { get; set; }
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4096;
    public double TopP { get; set; } = 1.0;
    public List<string>? StopSequences { get; set; }
    public Dictionary<string, object>? ExtraParameters { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
}

/// <summary>
/// Chat message roles
/// </summary>
public enum MessageRole
{
    System,
    User,
    Assistant,
    Tool
}

/// <summary>
/// Chat message structure
/// </summary>
public class ChatMessage
{
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? ToolCallId { get; set; }
}

/// <summary>
/// Tool/Function call definition
/// </summary>
public class ToolCall
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ArgumentsJson { get; set; } = "{}";
}

/// <summary>
/// Tool definition for function calling
/// </summary>
public class ToolDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ParametersJsonSchema { get; set; } = "{}";
}

/// <summary>
/// Tool call result
/// </summary>
public class ToolCallResult
{
    public string ToolCallId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsError { get; set; }
}

/// <summary>
/// Chat request
/// </summary>
public class ChatRequest
{
    public List<ChatMessage> Messages { get; set; } = new();
    public List<ToolDefinition>? Tools { get; set; }
    public AIRequestOptions Options { get; set; } = new();
}

/// <summary>
/// Chat response
/// </summary>
public class ChatResponse
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public MessageRole Role { get; set; } = MessageRole.Assistant;
    public List<ToolCall>? ToolCalls { get; set; }
    public int TotalTokens { get; set; }
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public double TotalCost { get; set; }
    public TimeSpan Latency { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Streaming chat delta
/// </summary>
public class ChatStreamDelta
{
    public string? Content { get; set; }
    public MessageRole? Role { get; set; }
    public ToolCall? ToolCall { get; set; }
    public bool IsComplete { get; set; }
    public ChatResponse? FinalResponse { get; set; }
}

/// <summary>
/// Embedding request
/// </summary>
public class EmbedRequest
{
    public List<string> Texts { get; set; } = new();
    public string? ModelId { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
}

/// <summary>
/// Embedding response
/// </summary>
public class EmbedResponse
{
    public List<EmbedResult> Embeddings { get; set; } = new();
    public string Model { get; set; } = string.Empty;
    public int TotalTokens { get; set; }
    public double TotalCost { get; set; }
    public TimeSpan Latency { get; set; }
}

/// <summary>
/// Single embedding result
/// </summary>
public class EmbedResult
{
    public string Text { get; set; } = string.Empty;
    public float[] Vector { get; set; } = Array.Empty<float>();
    public int Dimensions { get; set; }
}

/// <summary>
/// Model information
/// </summary>
public class ModelInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public int ContextLength { get; set; }
    public List<string> Capabilities { get; set; } = new();
    public double InputCostPer1KTokens { get; set; }
    public double OutputCostPer1KTokens { get; set; }
    public bool SupportsStreaming { get; set; }
    public bool SupportsFunctionCalling { get; set; }
    public bool SupportsVision { get; set; }
    public bool IsLocal { get; set; }
}

/// <summary>
/// Provider health status
/// </summary>
public class ProviderHealthStatus
{
    public string ProviderId { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public double LatencyMs { get; set; }
    public double LoadPercentage { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime LastChecked { get; set; }
}
