// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Ollama Provider Implementation
// ============================================================

using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Providers.Ollama;

// ============================================================
// OLLAMA DTOs
// ============================================================

public class OllamaChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("messages")]
    public List<OllamaMessage> Messages { get; set; } = new();
    
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = true;
    
    [JsonPropertyName("options")]
    public OllamaOptions? Options { get; set; }
    
    [JsonPropertyName("tools")]
    public List<OllamaTool>? Tools { get; set; }
}

public class OllamaMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonPropertyName("images")]
    public List<string>? Images { get; set; }
}

public class OllamaOptions
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.7;
    
    [JsonPropertyName("num_predict")]
    public int MaxTokens { get; set; } = 4096;
    
    [JsonPropertyName("top_p")]
    public double TopP { get; set; } = 1.0;
    
    [JsonPropertyName("stop")]
    public List<string>? Stop { get; set; }
}

public class OllamaTool
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";
    
    [JsonPropertyName("function")]
    public OllamaFunction Function { get; set; } = new();
}

public class OllamaFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("parameters")]
    public JsonElement Parameters { get; set; }
}

public class OllamaChatResponse
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    public OllamaMessage Message { get; set; } = new();
    
    [JsonPropertyName("done_reason")]
    public string? DoneReason { get; set; }
    
    [JsonPropertyName("total_duration")]
    public long TotalDuration { get; set; }
    
    [JsonPropertyName("load_duration")]
    public long LoadDuration { get; set; }
    
    [JsonPropertyName("prompt_eval_count")]
    public int PromptEvalCount { get; set; }
    
    [JsonPropertyName("eval_count")]
    public int EvalCount { get; set; }
}

public class OllamaEmbeddingRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;
}

public class OllamaEmbeddingResponse
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("embeddings")]
    public List<List<float>> Embeddings { get; set; } = new();
    
    [JsonPropertyName("total_duration")]
    public long TotalDuration { get; set; }
}

public class OllamaTagsResponse
{
    [JsonPropertyName("models")]
    public List<OllamaModelInfo> Models { get; set; } = new();
}

public class OllamaModelInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;
    
    [JsonPropertyName("size")]
    public long Size { get; set; }
    
    [JsonPropertyName("digest")]
    public string Digest { get; set; } = string.Empty;
    
    [JsonPropertyName("details")]
    public OllamaModelDetails Details { get; set; } = new();
}

public class OllamaModelDetails
{
    [JsonPropertyName("parent_model")]
    public string ParentModel { get; set; } = string.Empty;
    
    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;
    
    [JsonPropertyName("family")]
    public string Family { get; set; } = string.Empty;
    
    [JsonPropertyName("families")]
    public List<string> Families { get; set; } = new();
    
    [JsonPropertyName("parameter_size")]
    public string ParameterSize { get; set; } = string.Empty;
    
    [JsonPropertyName("quantization_level")]
    public string QuantizationLevel { get; set; } = string.Empty;
}

// ============================================================
// OLLAMA PROVIDER
// ============================================================

public class OllamaProvider : ILLMProvider, IEmbeddingProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaProvider> _logger;
    private readonly OllamaProviderConfig _config;
    
    // Model capability mappings
    private static readonly Dictionary<string, ModelInfo> _modelCapabilities = new()
    {
        ["llama3.2"] = new ModelInfo
        {
            Id = "llama3.2",
            Name = "Llama 3.2",
            Provider = "ollama",
            ContextLength = 128000,
            Capabilities = new List<string> { "chat", "reasoning", "function_calling" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = false,
            IsLocal = true
        },
        ["llama3.2:3b"] = new ModelInfo
        {
            Id = "llama3.2:3b",
            Name = "Llama 3.2 3B",
            Provider = "ollama",
            ContextLength = 128000,
            Capabilities = new List<string> { "chat", "reasoning", "function_calling" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = false,
            IsLocal = true
        },
        ["llama3.1"] = new ModelInfo
        {
            Id = "llama3.1",
            Name = "Llama 3.1",
            Provider = "ollama",
            ContextLength = 128000,
            Capabilities = new List<string> { "chat", "reasoning", "function_calling" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = true,
            SupportsFunctionCalling = true,
            SupportsVision = false,
            IsLocal = true
        },
        ["llama3"] = new ModelInfo
        {
            Id = "llama3",
            Name = "Llama 3",
            Provider = "ollama",
            ContextLength = 8192,
            Capabilities = new List<string> { "chat", "reasoning" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = true,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = true
        },
        ["phi3"] = new ModelInfo
        {
            Id = "phi3",
            Name = "Phi-3",
            Provider = "ollama",
            ContextLength = 4096,
            Capabilities = new List<string> { "chat", "fast" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = true,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = true
        },
        ["mistral"] = new ModelInfo
        {
            Id = "mistral",
            Name = "Mistral",
            Provider = "ollama",
            ContextLength = 8192,
            Capabilities = new List<string> { "chat", "reasoning" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = true,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = true
        },
        ["nomic-embed-text"] = new ModelInfo
        {
            Id = "nomic-embed-text",
            Name = "Nomic Embed Text",
            Provider = "ollama",
            ContextLength = 8192,
            Capabilities = new List<string> { "embeddings" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = false,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = true
        },
        ["mxbai-embed-large"] = new ModelInfo
        {
            Id = "mxbai-embed-large",
            Name = "MXBAI Embed Large",
            Provider = "ollama",
            ContextLength = 8192,
            Capabilities = new List<string> { "embeddings" },
            InputCostPer1KTokens = 0,
            OutputCostPer1KTokens = 0,
            SupportsStreaming = false,
            SupportsFunctionCalling = false,
            SupportsVision = false,
            IsLocal = true
        }
    };
    
    public string ProviderId => "ollama";
    public string DisplayName => "Ollama (Local LLM)";
    
    public OllamaProvider(HttpClient httpClient, ILogger<OllamaProvider> logger, OllamaProviderConfig config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = config;
        
        _httpClient.BaseAddress = new Uri(config.BaseUrl);
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
            var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
            status.IsHealthy = response.IsSuccessStatusCode;
            stopwatch.Stop();
            status.LatencyMs = stopwatch.ElapsedMilliseconds;
            
            if (!response.IsSuccessStatusCode)
            {
                status.ErrorMessage = $"Health check failed: {response.StatusCode}";
            }
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
        
        var ollamaRequest = new OllamaChatRequest
        {
            Model = modelId,
            Stream = false,
            Options = new OllamaOptions
            {
                Temperature = request.Options.Temperature,
                MaxTokens = request.Options.MaxTokens,
                TopP = request.Options.TopP,
                Stop = request.Options.StopSequences
            }
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
            
            ollamaRequest.Messages.Add(new OllamaMessage
            {
                Role = role,
                Content = message.Content
            });
        }
        
        if (request.Tools?.Any() == true)
        {
            ollamaRequest.Tools = request.Tools.Select(t => new OllamaTool
            {
                Function = new OllamaFunction
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = JsonDocument.Parse(t.ParametersJsonSchema).RootElement
                }
            }).ToList();
        }
        
        _logger.LogDebug("Sending chat request to Ollama: {Model}", modelId);
        
        var response = await _httpClient.PostAsJsonAsync("/api/chat", ollamaRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken);
        
        stopwatch.Stop();
        
        var toolCalls = new List<ToolCall>();
        if (ollamaResponse?.Message.ToolCalls != null)
        {
            foreach (var tc in ollamaResponse.Message.ToolCalls)
            {
                toolCalls.Add(new ToolCall
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = tc.Function.Name,
                    ArgumentsJson = tc.Function.Arguments.ToString() ?? "{}"
                });
            }
        }
        
        return new ChatResponse
        {
            Id = Guid.NewGuid().ToString(),
            Content = ollamaResponse?.Message.Content ?? string.Empty,
            Model = ollamaResponse?.Model ?? modelId,
            Role = MessageRole.Assistant,
            ToolCalls = toolCalls.Any() ? toolCalls : null,
            TotalTokens = (ollamaResponse?.PromptEvalCount ?? 0) + (ollamaResponse?.EvalCount ?? 0),
            PromptTokens = ollamaResponse?.PromptEvalCount ?? 0,
            CompletionTokens = ollamaResponse?.EvalCount ?? 0,
            TotalCost = 0, // Local = free
            Latency = stopwatch.Elapsed,
            Metadata = new Dictionary<string, string>
            {
                ["provider"] = "ollama",
                ["model"] = modelId,
                ["done_reason"] = ollamaResponse?.DoneReason ?? string.Empty
            }
        };
    }
    
    public async IAsyncEnumerable<ChatStreamDelta> StreamChatAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var modelId = request.Options.ModelId ?? _config.DefaultChatModel;
        
        var ollamaRequest = new OllamaChatRequest
        {
            Model = modelId,
            Stream = true,
            Options = new OllamaOptions
            {
                Temperature = request.Options.Temperature,
                MaxTokens = request.Options.MaxTokens,
                TopP = request.Options.TopP,
                Stop = request.Options.StopSequences
            }
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
            
            ollamaRequest.Messages.Add(new OllamaMessage
            {
                Role = role,
                Content = message.Content
            });
        }
        
        if (request.Tools?.Any() == true)
        {
            ollamaRequest.Tools = request.Tools.Select(t => new OllamaTool
            {
                Function = new OllamaFunction
                {
                    Name = t.Name,
                    Description = t.Description,
                    Parameters = JsonDocument.Parse(t.ParametersJsonSchema).RootElement
                }
            }).ToList();
        }
        
        var httpResponse = await _httpClient.PostAsJsonAsync("/api/chat", ollamaRequest, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        
        using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        
        var fullContent = new System.Text.StringBuilder();
        ChatResponse? finalResponse = null;
        
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrEmpty(line)) continue;
            
            try
            {
                var chunk = JsonSerializer.Deserialize<OllamaChatResponse>(line);
                if (chunk == null) continue;
                
                fullContent.Append(chunk.Message.Content);
                
                var delta = new ChatStreamDelta
                {
                    Content = chunk.Message.Content,
                    IsComplete = false
                };
                
                yield return delta;
                
                if (chunk.DoneReason != null)
                {
                    finalResponse = new ChatResponse
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = fullContent.ToString(),
                        Model = chunk.Model,
                        Role = MessageRole.Assistant,
                        TotalTokens = chunk.PromptEvalCount + chunk.EvalCount,
                        PromptTokens = chunk.PromptEvalCount,
                        CompletionTokens = chunk.EvalCount,
                        TotalCost = 0,
                        Metadata = new Dictionary<string, string>
                        {
                            ["done_reason"] = chunk.DoneReason
                        }
                    };
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse streaming response chunk");
            }
        }
        
        if (finalResponse != null)
        {
            yield return new ChatStreamDelta
            {
                IsComplete = true,
                FinalResponse = finalResponse
            };
        }
    }
    
    public async Task<List<ModelInfo>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/tags", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var tags = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>(cancellationToken: cancellationToken);
        
        var models = new List<ModelInfo>();
        foreach (var model in tags?.Models ?? new List<OllamaModelInfo>())
        {
            var baseModelName = model.Name.Split(':')[0];
            if (_modelCapabilities.TryGetValue(baseModelName, out var capability))
            {
                models.Add(new ModelInfo
                {
                    Id = model.Name,
                    Name = $"{capability.Name} ({FormatSize(model.Size)})",
                    Provider = ProviderId,
                    ContextLength = capability.ContextLength,
                    Capabilities = capability.Capabilities,
                    InputCostPer1KTokens = 0,
                    OutputCostPer1KTokens = 0,
                    SupportsStreaming = capability.SupportsStreaming,
                    SupportsFunctionCalling = capability.SupportsFunctionCalling,
                    SupportsVision = capability.SupportsVision,
                    IsLocal = true
                });
            }
            else
            {
                models.Add(new ModelInfo
                {
                    Id = model.Name,
                    Name = model.Name,
                    Provider = ProviderId,
                    ContextLength = 4096,
                    Capabilities = new List<string> { "chat" },
                    IsLocal = true
                });
            }
        }
        
        return models;
    }
    
    public ModelInfo? GetModelInfo(string modelId)
    {
        var baseModelName = modelId.Split(':')[0];
        if (_modelCapabilities.TryGetValue(baseModelName, out var capability))
        {
            return new ModelInfo
            {
                Id = modelId,
                Name = capability.Name,
                Provider = ProviderId,
                ContextLength = capability.ContextLength,
                Capabilities = capability.Capabilities,
                InputCostPer1KTokens = 0,
                OutputCostPer1KTokens = 0,
                SupportsStreaming = capability.SupportsStreaming,
                SupportsFunctionCalling = capability.SupportsFunctionCalling,
                SupportsVision = capability.SupportsVision,
                IsLocal = true
            };
        }
        
        return new ModelInfo
        {
            Id = modelId,
            Name = modelId,
            Provider = ProviderId,
            ContextLength = 4096,
            Capabilities = new List<string> { "chat" },
            IsLocal = true
        };
    }
    
    public async Task<EmbedResponse> EmbedAsync(EmbedRequest request, CancellationToken cancellationToken = default)
    {
        var modelId = request.ModelId ?? _config.DefaultEmbeddingModel;
        var stopwatch = Stopwatch.StartNew();
        
        var embeddings = new List<EmbedResult>();
        
        foreach (var text in request.Texts)
        {
            var ollamaRequest = new OllamaEmbeddingRequest
            {
                Model = modelId,
                Prompt = text
            };
            
            var response = await _httpClient.PostAsJsonAsync("/api/embeddings", ollamaRequest, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken);
            
            if (ollamaResponse?.Embeddings?.Any() == true)
            {
                var vector = ollamaResponse.Embeddings[0].ToArray();
                embeddings.Add(new EmbedResult
                {
                    Text = text,
                    Vector = vector,
                    Dimensions = vector.Length
                });
            }
        }
        
        stopwatch.Stop();
        
        return new EmbedResponse
        {
            Embeddings = embeddings,
            Model = modelId,
            TotalTokens = 0,
            TotalCost = 0,
            Latency = stopwatch.Elapsed
        };
    }
    
    public int GetEmbeddingDimensions(string modelId)
    {
        return modelId switch
        {
            "nomic-embed-text" => 768,
            "mxbai-embed-large" => 1024,
            _ => 768
        };
    }
    
    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Ollama provider configuration
/// </summary>
public class OllamaProviderConfig
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string DefaultChatModel { get; set; } = "llama3.2:3b";
    public string DefaultEmbeddingModel { get; set; } = "nomic-embed-text";
    public int TimeoutSeconds { get; set; } = 120;
    public int MaxRetries { get; set; } = 3;
}
