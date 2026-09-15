// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: OpenAI Provider Implementation
// ============================================================

using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Providers.OpenAI;

// ============================================================
// OPENAI DTOs
// ============================================================

public class OpenAIChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("messages")]
    public List<OpenAIMessage> Messages { get; set; } = new();
    
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.7;
    
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 4096;
    
    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 1.0;
    
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
    
    [JsonPropertyName("tools")]
    public List<OpenAITool>? Tools { get; set; }
    
    [JsonPropertyName("tool_choice")]
    public object? ToolChoice { get; set; }
}

public class OpenAIMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("tool_calls")]
    public List<OpenAIToolCall>? ToolCalls { get; set; }
    
    [JsonPropertyName("tool_call_id")]
    public string? ToolCallId { get; set; }
}

public class OpenAITool
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";
    
    [JsonPropertyName("function")]
    public OpenAIFunction Function { get; set; } = new();
}

public class OpenAIFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("parameters")]
    public JsonElement Parameters { get; set; }
}

public class OpenAIToolCall
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";
    
    [JsonPropertyName("function")]
    public OpenAIFunctionCall Function { get; set; } = new();
}

public class OpenAIFunctionCall
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; } = string.Empty;
}

public class OpenAIChatResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("choices")]
    public List<OpenAIChoice> Choices { get; set; } = new();
    
    [JsonPropertyName("usage")]
    public OpenAIUsage Usage { get; set; } = new();
    
    [JsonPropertyName("created")]
    public long Created { get; set; }
}

public class OpenAIChoice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    
    [JsonPropertyName("message")]
    public OpenAIMessage Message { get; set; } = new();
    
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

public class OpenAIUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }
    
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
    
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

public class OpenAIEmbeddingRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("input")]
    public List<string> Input { get; set; } = new();
}

public class OpenAIEmbeddingResponse
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("data")]
    public List<OpenAIEmbeddingData> Data { get; set; } = new();
    
    [JsonPropertyName("usage")]
    public OpenAIUsage Usage { get; set; } = new();
}

public class OpenAIEmbeddingData
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    
    [JsonPropertyName("embedding")]
    public List<float> Embedding { get; set; } = new();
    
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;
}

// ============================================================
// OPENAI PROVIDER
// ============================================================

public class OpenAIProvider : ILLMProvider, IEmbeddingProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIProvider> _logger;
    private readonly OpenAIProviderConfig _config;
    
    private static readonly Dictionary<string, ModelInfo> _modelCapabilities = new()
    {
        ["gpt-4o"] = new ModelInfo
        {
            Id = "gpt-4o",
            Name = "GPT-4o",
            Provider = "openai",
            ContextLength = 128000,
            Capabilities = new List<string> { "chat", "vision", "function_calling", "fast" },
            InputCostPer1KTokens = 0.005m,
            OutputCostPer1KTokens = 0.015m,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = true,
            IsLocal = false
        },
        ["gpt-4o-mini"] = new ModelInfo
        {
            Id = "gpt-4o-mini",
            Name = "GPT-4o Mini",
            Provider = "openai",
            ContextLength = 128000,
            Capabilities = new List<string> { "chat", "vision", "function_calling", "fast" },
            InputCostPer1KTokens = 0.00015m,
            OutputCostPer1KTokens = 0.0006m,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = true,
            IsLocal = false
        },
        ["gpt-4-turbo"] = new ModelInfo
        {
            Id = "gpt-4-turbo",
            Name = "GPT-4 Turbo",
            Provider = "openai",
            ContextLength = 128000,
            Capabilities = new List<string> { "chat", "vision", "function_calling" },
            InputCostPer1KTokens = 0.01m,
            OutputCostPer1KTokens = 0.03m,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = true,
            IsLocal = false
        },
        ["gpt-4"] = new ModelInfo
        {
            Id = "gpt-4",
            Name = "GPT-4",
            Provider = "openai",
            ContextLength = 8192,
            Capabilities = new List<string> { "chat", "function_calling" },
            InputCostPer1KTokens = 0.03m,
            OutputCostPer1KTokens = 0.06m,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = false,
            IsLocal = false
        },
        ["gpt-3.5-turbo"] = new ModelInfo
        {
            Id = "gpt-3.5-turbo",
            Name = "GPT-3.5 Turbo",
            Provider = "openai",
            ContextLength = 16385,
            Capabilities = new List<string> { "chat", "function_calling", "fast" },
            InputCostPer1KTokens = 0.0005m,
            OutputCostPer1KTokens = 0.0015m,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = false,
            IsLocal = false
        },
        ["text-embedding-3-small"] = new ModelInfo
        {
            Id = "text-embedding-3-small",
            Name = "Text Embedding 3 Small",
            Provider = "openai",
            ContextLength = 8191,
            Capabilities = new List<string> { "embeddings" },
            InputCostPer1KTokens = 0.00002m,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = false,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = false
        },
        ["text-embedding-3-large"] = new ModelInfo
        {
            Id = "text-embedding-3-large",
            Name = "Text Embedding 3 Large",
            Provider = "openai",
            ContextLength = 8191,
            Capabilities = new List<string> { "embeddings" },
            InputCostPer1KTokens = 0.00013m,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = false,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = false
        }
    };
    
    public string ProviderId => "openai";
    public string DisplayName => "OpenAI";
    
    public OpenAIProvider(HttpClient httpClient, ILogger<OpenAIProvider> logger, OpenAIProviderConfig config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = config;
        
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");
    }
    
    public async Task<ProviderHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default)
    {
        var status = new ProviderHealthStatus
        {
            ProviderId = ProviderId,
            LastChecked = DateTime.UtcNow
        };
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // Simple models list check
            var response = await _httpClient.GetAsync("/models/gpt-4o", cancellationToken);
            status.IsHealthy = response.IsSuccessStatusCode;
            stopwatch.Stop();
            status.LatencyMs = stopwatch.ElapsedMilliseconds;
        }
        catch (Exception ex)
        {
            status.IsHealthy = false;
            status.ErrorMessage = ex.Message;
            status.LatencyMs = stopwatch.ElapsedMilliseconds;
        }
        
        return status;
    }
    
    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var modelId = request.Options.ModelId ?? _config.DefaultChatModel;
        var stopwatch = Stopwatch.StartNew();
        
        var openaiRequest = new OpenAIChatRequest
        {
            Model = modelId,
            Temperature = request.Options.Temperature,
            MaxTokens = request.Options.MaxTokens,
            TopP = request.Options.TopP,
            Stream = false
        };
        
        foreach (var message in request.Messages)
        {
            var role = message.Role switch
            {
                MessageRole.System => "system",
                MessageRole.User => "user",
                MessageRole.Assistant => "assistant",
                MessageRole.Tool => "tool",
                _ => "user"
            };
            
            var openaiMessage = new OpenAIMessage
            {
                Role = role,
                Content = message.Content,
                Name = message.Name
            };
            
            openaiRequest.Messages.Add(openaiMessage);
        }
        
        if (request.Tools?.Any() == true)
        {
            openaiRequest.Tools = request.Tools.Select(t => new OpenAITool
            {
                Function = new OpenAIFunction
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = JsonDocument.Parse(t.ParametersJsonSchema).RootElement
                }
            }).ToList();
        }
        
        _logger.LogDebug("Sending chat request to OpenAI: {Model}", modelId);
        
        var response = await _httpClient.PostAsJsonAsync("/chat/completions", openaiRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var openaiResponse = await response.Content.ReadFromJsonAsync<OpenAIChatResponse>(cancellationToken: cancellationToken);
        
        stopwatch.Stop();
        
        var choice = openaiResponse?.Choices?.FirstOrDefault();
        var toolCalls = new List<ToolCall>();
        
        if (choice?.Message.ToolCalls != null)
        {
            foreach (var tc in choice.Message.ToolCalls)
            {
                toolCalls.Add(new ToolCall
                {
                    Id = tc.Id,
                    Name = tc.Function.Name,
                    ArgumentsJson = tc.Function.Arguments
                });
            }
        }
        
        var usage = openaiResponse?.Usage;
        var modelInfo = GetModelInfo(modelId);
        
        return new ChatResponse
        {
            Id = openaiResponse?.Id ?? Guid.NewGuid().ToString(),
            Content = choice?.Message.Content ?? string.Empty,
            Model = modelId,
            Role = MessageRole.Assistant,
            ToolCalls = toolCalls.Any() ? toolCalls : null,
            TotalTokens = usage?.TotalTokens ?? 0,
            PromptTokens = usage?.PromptTokens ?? 0,
            CompletionTokens = usage?.CompletionTokens ?? 0,
            TotalCost = CalculateCost(usage?.PromptTokens ?? 0, usage?.CompletionTokens ?? 0, modelInfo),
            Latency = stopwatch.Elapsed,
            Metadata = new Dictionary<string, string>
            {
                ["provider"] = "openai",
                ["model"] = modelId,
                ["finish_reason"] = choice?.FinishReason ?? string.Empty
            }
        };
    }
    
    public async IAsyncEnumerable<ChatStreamDelta> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var modelId = request.Options.ModelId ?? _config.DefaultChatModel;
        
        var openaiRequest = new OpenAIChatRequest
        {
            Model = modelId,
            Temperature = request.Options.Temperature,
            MaxTokens = request.Options.MaxTokens,
            TopP = request.Options.TopP,
            Stream = true
        };
        
        foreach (var message in request.Messages)
        {
            var role = message.Role switch
            {
                MessageRole.System => "system",
                MessageRole.User => "user",
                MessageRole.Assistant => "assistant",
                MessageRole.Tool => "tool",
                _ => "user"
            };
            
            openaiRequest.Messages.Add(new OpenAIMessage
            {
                Role = role,
                Content = message.Content,
                Name = message.Name
            });
        }
        
        if (request.Tools?.Any() == true)
        {
            openaiRequest.Tools = request.Tools.Select(t => new OpenAITool
            {
                Function = new OpenAIFunction
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = JsonDocument.Parse(t.ParametersJsonSchema).RootElement
                }
            }).ToList();
        }
        
        var httpResponse = await _httpClient.PostAsJsonAsync("/chat/completions", openaiRequest, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        
        using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        
        var fullContent = new System.Text.StringBuilder();
        
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrEmpty(line) || !line.StartsWith("data: ")) continue;
            
            if (line.Contains("[DONE]"))
            {
                yield break;
            }
            
            var json = line.Substring(6); // Remove "data: " prefix
            try
            {
                var chunk = JsonSerializer.Deserialize<OpenAIChatResponse>(json);
                var choice = chunk?.Choices?.FirstOrDefault();
                
                if (choice?.Message.Content != null)
                {
                    fullContent.Append(choice.Message.Content);
                    
                    yield return new ChatStreamDelta
                    {
                        Content = choice.Message.Content,
                        IsComplete = false
                    };
                }
                
                if (choice?.FinishReason != null)
                {
                    var modelInfo = GetModelInfo(modelId);
                    var usage = chunk?.Usage;
                    
                    yield return new ChatStreamDelta
                    {
                        IsComplete = true,
                        FinalResponse = new ChatResponse
                        {
                            Id = chunk?.Id ?? Guid.NewGuid().ToString(),
                            Content = fullContent.ToString(),
                            Model = modelId,
                            Role = MessageRole.Assistant,
                            TotalTokens = usage?.TotalTokens ?? 0,
                            PromptTokens = usage?.PromptTokens ?? 0,
                            CompletionTokens = usage?.CompletionTokens ?? 0,
                            TotalCost = CalculateCost(usage?.PromptTokens ?? 0, usage?.CompletionTokens ?? 0, modelInfo),
                            Metadata = new Dictionary<string, string>
                            {
                                ["finish_reason"] = choice.FinishReason
                            }
                        }
                    };
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse streaming response chunk");
            }
        }
    }
    
    public Task<List<ModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        var models = _modelCapabilities.Values.Select(m => new ModelInfo
        {
            Id = m.Id,
            Name = m.Name,
            Provider = ProviderId,
            ContextLength = m.ContextLength,
            Capabilities = m.Capabilities,
            InputCostPer1KTokens = m.InputCostPer1KTokens,
            OutputCostPer1KTokens = m.OutputCostPer1KTokens,
            SupportsStreaming = m.SupportsStreaming,
            SupportsFunctionCalling = m.SupportsFunctionCalling,
            SupportsVision = m.SupportsVision,
            IsLocal = m.IsLocal
        }).ToList();
        
        return Task.FromResult(models);
    }
    
    public ModelInfo? GetModelInfo(string modelId)
    {
        if (_modelCapabilities.TryGetValue(modelId, out var capability))
        {
            return capability;
        }
        
        return new ModelInfo
        {
            Id = modelId,
            Name = modelId,
            Provider = ProviderId,
            ContextLength = 4096,
            Capabilities = new List<string> { "chat" },
            IsLocal = false
        };
    }
    
    public async Task<EmbedResponse> EmbedAsync(EmbedRequest request, CancellationToken cancellationToken = default)
    {
        var modelId = request.ModelId ?? _config.DefaultEmbeddingModel;
        var stopwatch = Stopwatch.StartNew();
        
        var openaiRequest = new OpenAIEmbeddingRequest
        {
            Model = modelId,
            Input = request.Texts
        };
        
        var response = await _httpClient.PostAsJsonAsync("/embeddings", openaiRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var openaiResponse = await response.Content.ReadFromJsonAsync<OpenAIEmbeddingResponse>(cancellationToken: cancellationToken);
        
        stopwatch.Stop();
        
        var embeddings = new List<EmbedResult>();
        foreach (var data in openaiResponse?.Data ?? new List<OpenAIEmbeddingData>())
        {
            embeddings.Add(new EmbedResult
            {
                Text = request.Texts[data.Index],
                Vector = data.Embedding.ToArray(),
                Dimensions = data.Embedding.Count
            });
        }
        
        var usage = openaiResponse?.Usage;
        var modelInfo = _modelCapabilities.GetValueOrDefault(modelId);
        
        return new EmbedResponse
        {
            Embeddings = embeddings,
            Model = modelId,
            TotalTokens = usage?.TotalTokens ?? 0,
            TotalCost = (usage?.PromptTokens ?? 0) * (modelInfo?.InputCostPer1KTokens ?? 0) / 1000,
            Latency = stopwatch.Elapsed
        };
    }
    
    public int GetEmbeddingDimensions(string modelId)
    {
        return modelId switch
        {
            "text-embedding-3-large" => 3072,
            "text-embedding-3-small" => 1536,
            _ => 1536
        };
    }
    
    private static decimal CalculateCost(int promptTokens, int completionTokens, ModelInfo? modelInfo)
    {
        if (modelInfo == null) return 0;
        
        return (promptTokens * modelInfo.InputCostPer1KTokens + 
                completionTokens * modelInfo.OutputCostPer1KTokens) / 1000;
    }
}

/// <summary>
/// OpenAI provider configuration
/// </summary>
public class OpenAIProviderConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string DefaultChatModel { get; set; } = "gpt-4o-mini";
    public string DefaultEmbeddingModel { get; set; } = "text-embedding-3-small";
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    public int TimeoutSeconds { get; set; } = 60;
    public int MaxRetries { get; set; } = 3;
}
