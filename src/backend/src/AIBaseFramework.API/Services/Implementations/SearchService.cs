// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Search Service Implementation
// ============================================================

using System.Diagnostics;
using AIBaseFramework.API.Infrastructure.AI;
using AIBaseFramework.API.Infrastructure.Data;
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIBaseFramework.API.Services.Implementations;

public class SearchService : ISearchService
{
    private readonly AppDbContext _dbContext;
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<SearchService> _logger;

    public SearchService(
        AppDbContext dbContext,
        IVectorStore vectorStore,
        IEmbeddingService embeddingService,
        ILogger<SearchService> logger)
    {
        _dbContext = dbContext;
        _vectorStore = vectorStore;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task<List<SearchResult>> SearchAsync(
        string query, 
        Guid userId, 
        int topK = 10, 
        Guid? categoryId = null, 
        DateTime? fromDate = null, 
        DateTime? toDate = null)
    {
        var stopwatch = Stopwatch.StartNew();

        // Generate query embedding
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);

        // Search vector store
        var searchRequest = new VectorSearchRequest
        {
            QueryVector = queryEmbedding,
            TopK = topK,
            MinScore = 0.5,
            TenantId = userId.ToString(),
            FilterDocumentIds = categoryId.HasValue 
                ? new List<string> { categoryId.Value.ToString() }
                : null
        };

        var vectorResults = await _vectorStore.SearchAsync(searchRequest);

        // Apply date filters if specified
        var results = vectorResults.Results.Select(r => new SearchResult(
            Guid.TryParse(r.DocumentId, out var docId) ? docId : Guid.Empty,
            r.Metadata.GetValueOrDefault("title", "Untitled"),
            r.Content.Length > 300 ? r.Content.Substring(0, 300) + "..." : r.Content,
            (float)r.Score,
            null,
            r.Metadata.GetValueOrDefault("url")
        )).ToList();

        // Log search
        var searchLog = new SearchLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            QueryText = query,
            QueryEmbedding = queryEmbedding,
            ResultsCount = results.Count,
            LatencyMs = (int)stopwatch.ElapsedMilliseconds,
            SearchType = "semantic",
            TopResults = "[]",
            UserFeedback = "{}",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.SearchLogs.Add(searchLog);
        await _dbContext.SaveChangesAsync();

        stopwatch.Stop();
        _logger.LogInformation("Search completed: {Query}, {Count} results in {Time}ms", 
            query, results.Count, stopwatch.ElapsedMilliseconds);

        return results;
    }

    public async Task<List<SearchResult>> HybridSearchAsync(
        string query, 
        Guid userId, 
        int topK = 10, 
        float vectorWeight = 0.7f, 
        float keywordWeight = 0.3f)
    {
        var stopwatch = Stopwatch.StartNew();

        // Vector search
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
        var vectorResults = await _vectorStore.SearchAsync(new VectorSearchRequest
        {
            QueryVector = queryEmbedding,
            TopK = topK * 2,
            MinScore = 0.3,
            TenantId = userId.ToString()
        });

        // Keyword search
        var keywordResults = await _dbContext.DocumentChunks
            .Include(c => c.Document)
            .Where(c => c.IsActive && c.Document != null)
            .Where(c => EF.Functions.ILike(c.Content, $"%{query}%"))
            .Take(topK * 2)
            .Select(c => new
            {
                c.DocumentId,
                Title = c.Document != null ? c.Document.Title : "Untitled",
                c.Content,
                c.PageNumber
            })
            .ToListAsync();

        // Merge and rerank results
        var mergedResults = new Dictionary<string, SearchResult>();

        // Add vector results
        foreach (var r in vectorResults.Results)
        {
            var excerpt = r.Content.Length > 300 ? r.Content.Substring(0, 300) + "..." : r.Content;
            mergedResults[r.ChunkId] = new SearchResult(
                Guid.TryParse(r.DocumentId, out var docId) ? docId : Guid.Empty,
                r.Metadata.GetValueOrDefault("title", "Untitled"),
                excerpt,
                (float)r.Score * vectorWeight,
                null,
                null);
        }

        // Add/merge keyword results
        foreach (var r in keywordResults)
        {
            var excerpt = r.Content.Length > 300 ? r.Content.Substring(0, 300) + "..." : r.Content;
            var key = r.DocumentId.ToString();
            
            if (mergedResults.TryGetValue(key, out var existing))
            {
                // Boost score for keyword match
                mergedResults[key] = new SearchResult(
                    existing.DocumentId,
                    existing.Title,
                    existing.Excerpt,
                    existing.Score + keywordWeight,
                    existing.PageNumber,
                    existing.Source);
            }
            else
            {
                mergedResults[key] = new SearchResult(
                    r.DocumentId,
                    r.Title,
                    excerpt,
                    keywordWeight,
                    r.PageNumber,
                    null);
            }
        }

        var results = mergedResults.Values
            .OrderByDescending(r => r.Score)
            .Take(topK)
            .ToList();

        // Log search
        var searchLog = new SearchLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            QueryText = query,
            QueryEmbedding = queryEmbedding,
            ResultsCount = results.Count,
            LatencyMs = (int)stopwatch.ElapsedMilliseconds,
            SearchType = "hybrid",
            TopResults = "[]",
            UserFeedback = "{}",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.SearchLogs.Add(searchLog);
        await _dbContext.SaveChangesAsync();

        stopwatch.Stop();
        _logger.LogInformation("Hybrid search completed: {Query}, {Count} results in {Time}ms",
            query, results.Count, stopwatch.ElapsedMilliseconds);

        return results;
    }
}

// Vector Search DTOs
public class VectorSearchRequest
{
    public float[] QueryVector { get; set; } = Array.Empty<float>();
    public int TopK { get; set; } = 10;
    public double MinScore { get; set; } = 0.5;
    public string? TenantId { get; set; }
    public List<string>? FilterDocumentIds { get; set; }
}

public class VectorSearchResult
{
    public string ChunkId { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class VectorSearchResponse
{
    public List<VectorSearchResult> Results { get; set; } = new();
    public long TotalResults { get; set; }
    public long SearchTimeMs { get; set; }
}

public interface IVectorStore
{
    Task<VectorSearchResponse> SearchAsync(VectorSearchRequest request);
    Task<List<VectorSearchResult>> GetChunksByDocumentIdAsync(Guid documentId);
    Task DeleteChunksByDocumentIdAsync(Guid documentId);
}
