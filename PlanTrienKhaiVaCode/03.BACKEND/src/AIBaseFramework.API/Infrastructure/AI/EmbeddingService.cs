// Embedding Service
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIBaseFramework.API.Infrastructure.AI;

public class EmbeddingService : IEmbeddingService
{
    private readonly IOllamaService _ollamaService;
    private readonly IRedisCacheService _cacheService;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        IOllamaService ollamaService,
        IRedisCacheService cacheService,
        ILogger<EmbeddingService> logger)
    {
        _ollamaService = ollamaService;
        _cacheService = cacheService;
        _logger = logger;
    }

    public int EmbeddingDimensions => 768; // Ollama nomic-embed-text

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        // Check cache first
        var cacheKey = $"embedding:{ComputeHash(text)}";
        var cached = await _cacheService.GetAsync<float[]>(cacheKey);
        
        if (cached != null)
        {
            _logger.LogDebug("Embedding cache hit for text hash: {Hash}", cacheKey);
            return cached;
        }

        // Generate embedding from Ollama
        var embedding = await _ollamaService.EmbedAsync(text);

        // Cache for 30 days
        await _cacheService.SetAsync(cacheKey, embedding, TimeSpan.FromDays(30));

        _logger.LogDebug("Generated new embedding, dimensions: {Dimensions}", embedding.Length);
        return embedding;
    }

    public async Task<List<float[]>> GenerateBatchEmbeddingsAsync(List<string> texts)
    {
        var results = new List<float[]>();

        foreach (var text in texts)
        {
            var embedding = await GenerateEmbeddingAsync(text);
            results.Add(embedding);
        }

        return results;
    }

    private static string ComputeHash(string text)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash)[..16];
    }
}
