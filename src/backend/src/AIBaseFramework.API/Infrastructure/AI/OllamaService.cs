// Ollama Service - Local LLM Integration
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIBaseFramework.API.Infrastructure.AI;

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaService> _logger;
    private readonly string _baseUrl;
    private readonly string _chatModel;
    private readonly string _embeddingModel;
    private readonly float _temperature;
    private readonly int _maxTokens;

    public OllamaService(
        HttpClient httpClient,
        ILogger<OllamaService> logger,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["OllamaConfig:BaseUrl"] ?? "http://localhost:11434";
        _chatModel = configuration["OllamaConfig:ChatModel"] ?? "llama3.2:3b";
        _embeddingModel = configuration["OllamaConfig:EmbeddingModel"] ?? "nomic-embed-text";
        _temperature = float.Parse(configuration["OllamaConfig:Temperature"] ?? "0.3");
        _maxTokens = int.Parse(configuration["OllamaConfig:MaxTokens"] ?? "2000");
    }

    public async Task<string> ChatAsync(string prompt, string? systemPrompt = null)
    {
        var messages = new List<ChatMessage>();
        
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            messages.Add(new ChatMessage { Role = "system", Content = systemPrompt });
        }
        
        messages.Add(new ChatMessage { Role = "user", Content = prompt });

        var request = new OllamaChatRequest
        {
            Model = _chatModel,
            Messages = messages,
            Stream = false,
            Options = new OllamaOptions
            {
                Temperature = _temperature,
                NumPredict = _maxTokens
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_baseUrl}/api/chat",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>();
            
            if (result?.Message?.Content == null)
            {
                throw new Exception("Ollama returned empty response");
            }

            _logger.LogInformation("Chat completion successful, model: {Model}", _chatModel);
            return result.Message.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama chat failed for prompt: {Prompt}", prompt);
            throw;
        }
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var request = new OllamaEmbedRequest
        {
            Model = _embeddingModel,
            Input = text
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_baseUrl}/api/embeddings",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>();

            if (result?.Embedding == null || result.Embedding.Length == 0)
            {
                throw new Exception("Ollama returned empty embedding");
            }

            _logger.LogDebug("Embedding generated, dimensions: {Dimensions}", result.Embedding.Length);
            return result.Embedding;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama embedding failed for text: {Text}", text);
            throw;
        }
    }

    public async Task<List<string>> ListModelsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/tags");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>();
            return result?.Models?.Select(m => m.Name).ToList() ?? new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list Ollama models");
            return new List<string>();
        }
    }
}

// DTOs for Ollama API
public class OllamaChatRequest
{
    public string Model { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();
    public bool Stream { get; set; } = false;
    public OllamaOptions? Options { get; set; }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class OllamaOptions
{
    public float Temperature { get; set; } = 0.3f;
    public int NumPredict { get; set; } = 2000;
}

public class OllamaChatResponse
{
    public ChatMessageResponse? Message { get; set; }
    public string? Model { get; set; }
}

public class ChatMessageResponse
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class OllamaEmbedRequest
{
    public string Model { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
}

public class OllamaEmbedResponse
{
    public float[] Embedding { get; set; } = Array.Empty<float>();
}

public class OllamaTagsResponse
{
    public List<OllamaModelInfo>? Models { get; set; }
}

public class OllamaModelInfo
{
    public string Name { get; set; } = string.Empty;
    public string? ModifiedAt { get; set; }
    public long? Size { get; set; }
}
