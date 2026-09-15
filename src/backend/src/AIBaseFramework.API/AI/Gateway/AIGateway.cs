// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: AI Gateway - Multi-Provider Router & Manager
// ============================================================

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Gateway;

// ============================================================
// AI GATEWAY - Unified Interface for All AI Providers
// ============================================================

/// <summary>
/// AI Gateway - Routes requests to appropriate AI providers
/// with support for failover, load balancing, and cost optimization
/// </summary>
public interface IAI Gateway
{
    // Chat operations
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ChatStreamDelta> StreamChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    // Embedding operations
    Task<EmbedResponse> EmbedAsync(EmbedRequest request, CancellationToken cancellationToken = default);
    
    // Provider management
    Task<ChatResponse> ChatWithProviderAsync(string providerId, ChatRequest request, CancellationToken cancellationToken = default);
    Task<EmbedResponse> EmbedWithProviderAsync(string providerId, EmbedRequest request, CancellationToken cancellationToken = default);
    
    // Health & Status
    Task<Dictionary<string, ProviderHealthStatus>> GetAllProviderHealthAsync(CancellationToken cancellationToken = default);
    Task<List<ModelInfo>> GetAllAvailableModelsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// AI Gateway implementation with routing strategies
/// </summary>
public class AI Gateway : IAI Gateway
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<ILLMProvider> _llmProviders;
    private readonly IEnumerable<IEmbeddingProvider> _embeddingProviders;
    private readonly AI GatewayConfig _config;
    private readonly ILogger<AI Gateway> _logger;
    
    public AI Gateway(
        IServiceProvider serviceProvider,
        IEnumerable<ILLMProvider> llmProviders,
        IEnumerable<IEmbeddingProvider> embeddingProviders,
        AI GatewayConfig config,
        ILogger<AI Gateway> logger)
    {
        _serviceProvider = serviceProvider;
        _llmProviders = llmProviders;
        _embeddingProviders = embeddingProviders;
        _config = config;
        _logger = logger;
    }
    
    /// <summary>
    /// Chat with automatic provider selection based on routing strategy
    /// </summary>
    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var strategy = _config.DefaultRoutingStrategy;
        
        return strategy switch
        {
            RoutingStrategy.CostOptimized => await ChatWithCostOptimizationAsync(request, cancellationToken),
            RoutingStrategy.LatencyOptimized => await ChatWithLatencyOptimizationAsync(request, cancellationToken),
            RoutingStrategy.Fallback => await ChatWithFallbackAsync(request, cancellationToken),
            RoutingStrategy.Specific => await ChatWithSpecificProviderAsync(request, cancellationToken),
            _ => await ChatWithFallbackAsync(request, cancellationToken)
        };
    }
    
    /// <summary>
    /// Streaming chat with automatic provider selection
    /// </summary>
    public async IAsyncEnumerable<ChatStreamDelta> StreamChatAsync(
        ChatRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var provider = await SelectBestLLMProviderAsync(request, cancellationToken);
        
        if (provider == null)
        {
            _logger.LogError("No available LLM provider found");
            yield break;
        }
        
        _logger.LogInformation("Streaming chat with provider: {Provider}", provider.ProviderId);
        
        await foreach (var delta in provider.StreamChatAsync(request, cancellationToken))
        {
            yield return delta;
        }
    }
    
    /// <summary>
    /// Embed with automatic provider selection
    /// </summary>
    public async Task<EmbedResponse> EmbedAsync(EmbedRequest request, CancellationToken cancellationToken = default)
    {
        var provider = SelectEmbeddingProvider(request);
        
        if (provider == null)
        {
            throw new InvalidOperationException("No embedding provider available");
        }
        
        _logger.LogDebug("Embedding with provider: {Provider}", provider.ProviderId);
        
        return await provider.EmbedAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Chat with specific provider
    /// </summary>
    public async Task<ChatResponse> ChatWithProviderAsync(
        string providerId, 
        ChatRequest request, 
        CancellationToken cancellationToken = default)
    {
        var provider = _llmProviders.FirstOrDefault(p => 
            p.ProviderId.Equals(providerId, StringComparison.OrdinalIgnoreCase));
        
        if (provider == null)
        {
            throw new ArgumentException($"Provider not found: {providerId}");
        }
        
        _logger.LogDebug("Chat with specific provider: {Provider}", providerId);
        
        return await provider.ChatAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Embed with specific provider
    /// </summary>
    public async Task<EmbedResponse> EmbedWithProviderAsync(
        string providerId, 
        EmbedRequest request, 
        CancellationToken cancellationToken = default)
    {
        var provider = _embeddingProviders.FirstOrDefault(p => 
            p.ProviderId.Equals(providerId, StringComparison.OrdinalIgnoreCase));
        
        if (provider == null)
        {
            throw new ArgumentException($"Embedding provider not found: {providerId}");
        }
        
        return await provider.EmbedAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Get health status of all providers
    /// </summary>
    public async Task<Dictionary<string, ProviderHealthStatus>> GetAllProviderHealthAsync(
        CancellationToken cancellationToken = default)
    {
        var healthStatus = new Dictionary<string, ProviderHealthStatus>();
        
        var llmTasks = _llmProviders.Select(p => 
            GetHealthWithFallback(p, healthStatus, cancellationToken));
        
        var embedTasks = _embeddingProviders.Select(p => 
            GetEmbeddingHealthWithFallback(p, healthStatus, cancellationToken));
        
        await Task.WhenAll(llmTasks.Concat(embedTasks));
        
        return healthStatus;
    }
    
    /// <summary>
    /// Get all available models from all providers
    /// </summary>
    public async Task<List<ModelInfo>> GetAllAvailableModelsAsync(
        CancellationToken cancellationToken = default)
    {
        var allModels = new List<ModelInfo>();
        
        var llmTasks = _llmProviders.Select(async p =>
        {
            try
            {
                return await p.GetAvailableModelsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get models from provider: {Provider}", p.ProviderId);
                return new List<ModelInfo>();
            }
        });
        
        var embedTasks = _embeddingProviders.Select(async p =>
        {
            try
            {
                var models = await p.GetAvailableModelsAsync(cancellationToken);
                return models.Where(m => m.Capabilities.Contains("embeddings")).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get embedding models from provider: {Provider}", p.ProviderId);
                return new List<ModelInfo>();
            }
        });
        
        var results = await Task.WhenAll(llmTasks.Concat(embedTasks));
        
        foreach (var models in results)
        {
            allModels.AddRange(models);
        }
        
        return allModels;
    }
    
    // ============================================================
    // ROUTING STRATEGIES
    // ============================================================
    
    /// <summary>
    /// Cost-optimized routing - prefer free local providers
    /// </summary>
    private async Task<ChatResponse> ChatWithCostOptimizationAsync(
        ChatRequest request, 
        CancellationToken cancellationToken)
    {
        // Priority: Ollama (free) > Cheap providers > Expensive providers
        var orderedProviders = _llmProviders
            .OrderBy(p => GetProviderBaseCost(p.ProviderId))
            .ToList();
        
        foreach (var provider in orderedProviders)
        {
            try
            {
                var health = await provider.GetHealthStatusAsync(cancellationToken);
                if (health.IsHealthy)
                {
                    _logger.LogDebug("Using cost-optimized provider: {Provider}", provider.ProviderId);
                    return await provider.ChatAsync(request, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Provider {Provider} failed cost-optimized routing", provider.ProviderId);
            }
        }
        
        throw new InvalidOperationException("No available provider for cost-optimized routing");
    }
    
    /// <summary>
    /// Latency-optimized routing - prefer fastest responding provider
    /// </summary>
    private async Task<ChatResponse> ChatWithLatencyOptimizationAsync(
        ChatRequest request, 
        CancellationToken cancellationToken)
    {
        var providerTasks = _llmProviders.Select(async p =>
        {
            var health = await p.GetHealthStatusAsync(cancellationToken);
            return new { Provider = p, Health = health };
        });
        
        var results = await Task.WhenAll(providerTasks);
        
        var bestProvider = results
            .Where(r => r.Health.IsHealthy)
            .OrderBy(r => r.Health.LatencyMs)
            .FirstOrDefault()?.Provider;
        
        if (bestProvider == null)
        {
            throw new InvalidOperationException("No healthy provider found for latency-optimized routing");
        }
        
        _logger.LogDebug("Using latency-optimized provider: {Provider}", bestProvider.ProviderId);
        return await bestProvider.ChatAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Fallback routing - use primary, fallback to secondary on failure
    /// </summary>
    private async Task<ChatResponse> ChatWithFallbackAsync(
        ChatRequest request, 
        CancellationToken cancellationToken)
    {
        var providers = _config.FallbackProviderOrder.Count > 0
            ? _llmProviders.Where(p => 
                _config.FallbackProviderOrder.Contains(p.ProviderId))
                .OrderBy(p => _config.FallbackProviderOrder.IndexOf(p.ProviderId))
            : _llmProviders;
        
        Exception? lastException = null;
        
        foreach (var provider in providers)
        {
            try
            {
                var health = await provider.GetHealthStatusAsync(cancellationToken);
                if (!health.IsHealthy)
                {
                    _logger.LogWarning("Provider {Provider} is not healthy, skipping", provider.ProviderId);
                    continue;
                }
                
                _logger.LogDebug("Using fallback provider: {Provider}", provider.ProviderId);
                return await provider.ChatAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fallback provider {Provider} failed", provider.ProviderId);
                lastException = ex;
            }
        }
        
        throw new InvalidOperationException("All fallback providers failed", lastException);
    }
    
    /// <summary>
    /// Use specific provider from request options
    /// </summary>
    private async Task<ChatResponse> ChatWithSpecificProviderAsync(
        ChatRequest request, 
        CancellationToken cancellationToken)
    {
        var providerId = request.Options.ExtraParameters?.GetValueOrDefault("provider")?.ToString();
        
        if (string.IsNullOrEmpty(providerId))
        {
            return await ChatWithFallbackAsync(request, cancellationToken);
        }
        
        var provider = _llmProviders.FirstOrDefault(p => 
            p.ProviderId.Equals(providerId, StringComparison.OrdinalIgnoreCase));
        
        if (provider == null)
        {
            throw new ArgumentException($"Specified provider not found: {providerId}");
        }
        
        return await provider.ChatAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Select best LLM provider based on strategy
    /// </summary>
    private async Task<ILLMProvider?> SelectBestLLMProviderAsync(
        ChatRequest request, 
        CancellationToken cancellationToken)
    {
        var strategy = _config.DefaultRoutingStrategy;
        
        return strategy switch
        {
            RoutingStrategy.CostOptimized => 
                _llmProviders
                    .OrderBy(p => GetProviderBaseCost(p.ProviderId))
                    .FirstOrDefault(p => p.GetHealthStatusAsync(cancellationToken).Result.IsHealthy),
                    
            RoutingStrategy.LatencyOptimized => 
                (await Task.WhenAll(_llmProviders.Select(async p => 
                    new { Provider = p, Health = await p.GetHealthStatusAsync(cancellationToken) })))
                    .Where(r => r.Health.IsHealthy)
                    .OrderBy(r => r.Health.LatencyMs)
                    .FirstOrDefault()?.Provider,
                    
            _ => _llmProviders.FirstOrDefault()
        };
    }
    
    /// <summary>
    /// Select embedding provider
    /// </summary>
    private IEmbeddingProvider? SelectEmbeddingProvider(EmbedRequest request)
    {
        // Prefer local providers for embeddings (cheaper, faster)
        var localProvider = _embeddingProviders
            .FirstOrDefault(p => p.ProviderId == "ollama");
        
        if (localProvider != null)
        {
            return localProvider;
        }
        
        return _embeddingProviders.FirstOrDefault();
    }
    
    private async Task GetHealthWithFallback(
        ILLMProvider provider, 
        Dictionary<string, ProviderHealthStatus> healthStatus,
        CancellationToken cancellationToken)
    {
        try
        {
            var health = await provider.GetHealthStatusAsync(cancellationToken);
            healthStatus[provider.ProviderId] = health;
        }
        catch (Exception ex)
        {
            healthStatus[provider.ProviderId] = new ProviderHealthStatus
            {
                ProviderId = provider.ProviderId,
                IsHealthy = false,
                ErrorMessage = ex.Message,
                LastChecked = DateTime.UtcNow
            };
        }
    }
    
    private async Task GetEmbeddingHealthWithFallback(
        IEmbeddingProvider provider, 
        Dictionary<string, ProviderHealthStatus> healthStatus,
        CancellationToken cancellationToken)
    {
        try
        {
            var health = await provider.GetHealthStatusAsync(cancellationToken);
            healthStatus[$"{provider.ProviderId}-embed"] = health;
        }
        catch (Exception ex)
        {
            healthStatus[$"{provider.ProviderId}-embed"] = new ProviderHealthStatus
            {
                ProviderId = $"{provider.ProviderId}-embed",
                IsHealthy = false,
                ErrorMessage = ex.Message,
                LastChecked = DateTime.UtcNow
            };
        }
    }
    
    private static decimal GetProviderBaseCost(string providerId)
    {
        return providerId.ToLower() switch
        {
            "ollama" => 0m,           // Free (local)
            "groq" => 0.0001m,       // Very cheap
            "deepseek" => 0.0002m,   // Cheap
            "openai" => 0.001m,      // Medium
            "anthropic" => 0.003m,   // Expensive
            "azure" => 0.002m,       // Medium-expensive
            _ => 0.001m
        };
    }
}

// ============================================================
// ENUMS & CONFIG
// ============================================================

public enum RoutingStrategy
{
    /// <summary>
    /// Use cheapest available provider (prefer local)
    /// </summary>
    CostOptimized,
    
    /// <summary>
    /// Use fastest responding provider
    /// </summary>
    LatencyOptimized,
    
    /// <summary>
    /// Use primary, fallback to secondary on failure
    /// </summary>
    Fallback,
    
    /// <summary>
    /// Use specific provider from request
    /// </summary>
    Specific
}

public class AIGatewayConfig
{
    public RoutingStrategy DefaultRoutingStrategy { get; set; } = RoutingStrategy.Fallback;
    public List<string> FallbackProviderOrder { get; set; } = new() { "ollama", "openai", "anthropic" };
    public bool EnableCostTracking { get; set; } = true;
    public bool EnableLatencyTracking { get; set; } = true;
    public int HealthCheckIntervalSeconds { get; set; } = 60;
    public double LatencyThresholdMs { get; set; } = 5000;
}
