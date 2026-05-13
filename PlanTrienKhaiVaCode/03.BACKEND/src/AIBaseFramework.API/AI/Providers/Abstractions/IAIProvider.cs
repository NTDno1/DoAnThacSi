// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: AI Provider Abstraction Layer - Core Interface
// ============================================================

using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Providers.Abstractions;

/// <summary>
/// Unified interface for all AI providers (LLM and Embedding)
/// </summary>
public interface IAIProvider
{
    /// <summary>
    /// Provider identifier (e.g., "openai", "anthropic", "ollama")
    /// </summary>
    string ProviderId { get; }
    
    /// <summary>
    /// Provider display name
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// Check if provider is currently healthy/available
    /// </summary>
    Task<ProviderHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// LLM Provider interface for chat completions
/// </summary>
public interface ILLMProvider : IAIProvider
{
    /// <summary>
    /// Send chat completion request
    /// </summary>
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Stream chat completion (for real-time responses)
    /// </summary>
    IAsyncEnumerable<ChatStreamDelta> StreamChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get available models from this provider
    /// </summary>
    Task<List<ModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get model info by ID
    /// </summary>
    ModelInfo? GetModelInfo(string modelId);
}

/// <summary>
/// Embedding Provider interface for text embeddings
/// </summary>
public interface IEmbeddingProvider : IAIProvider
{
    /// <summary>
    /// Generate embeddings for texts
    /// </summary>
    Task<EmbedResponse> EmbedAsync(EmbedRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get embedding dimensions for this provider
    /// </summary>
    int GetEmbeddingDimensions(string modelId);
}
