// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Semantic Search & Hybrid Search Engine
// ============================================================

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Gateway;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.Search;

// ============================================================
// SEARCH TYPES
// ============================================================

/// <summary>
/// Search request
/// </summary>
public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public SearchType SearchType { get; set; } = SearchType.Hybrid;
    public int Limit { get; set; } = 20;
    public double MinRelevanceScore { get; set; } = 0.5;
    public List<string>? FilterDocumentIds { get; set; }
    public Dictionary<string, string>? MetadataFilters { get; set; }
    public bool IncludeHighlights { get; set; } = true;
    public bool IncludeFacets { get; set; } = false;
}

/// <summary>
/// Search type
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Pure vector similarity search
    /// </summary>
    Semantic,
    
    /// <summary>
    /// Traditional keyword search
    /// </summary>
    Keyword,
    
    /// <summary>
    /// Combined semantic + keyword (default)
    /// </summary>
    Hybrid,
    
    /// <summary>
    /// AI-powered intelligent search
    /// </summary>
    AI
}

/// <summary>
/// Search result
/// </summary>
public class SearchResult
{
    public string DocumentId { get; set; } = string.Empty;
    public string ChunkId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Url { get; set; }
    public double Score { get; set; }
    public double SemanticScore { get; set; }
    public double KeywordScore { get; set; }
    public List<string> Highlights { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
    public int PageNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Search response
/// </summary>
public class SearchResponse
{
    public List<SearchResult> Results { get; set; } = new();
    public string Query { get; set; } = string.Empty;
    public SearchType SearchType { get; set; }
    public int TotalResults { get; set; }
    public int RetrievedResults { get; set; }
    public TimeSpan SearchTime { get; set; }
    public List<SearchFacet>? Facets { get; set; }
    public Dictionary<string, object>? Analytics { get; set; }
}

/// <summary>
/// Search facet for filtering
/// </summary>
public class SearchFacet
{
    public string Name { get; set; } = string.Empty;
    public List<FacetValue> Values { get; set; } = new();
}

/// <summary>
/// Facet value with count
/// </summary>
public class FacetValue
{
    public string Value { get; set; } = string.Empty;
    public int Count { get; set; }
    public bool IsSelected { get; set; }
}

// ============================================================
// SEARCH ENGINE
// ============================================================

/// <summary>
/// Search engine interface
/// </summary>
public interface ISearchEngine
{
    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
    Task<SearchResponse> SemanticSearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
    Task<SearchResponse> KeywordSearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
    Task<SearchResponse> HybridSearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
    Task<SearchResponse> AISearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
    Task<List<string>> AutocompleteAsync(string prefix, int limit = 10, CancellationToken cancellationToken = default);
}

/// <summary>
/// Search engine implementation with multiple search strategies
/// </summary>
public class SearchEngine : ISearchEngine
{
    private readonly IAI Gateway _aiGateway;
    private readonly IVectorStore _vectorStore;
    private readonly ISearchRepository _searchRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<SearchEngine> _logger;
    private readonly SearchEngineConfig _config;
    
    public SearchEngine(
        IAI Gateway aiGateway,
        IVectorStore vectorStore,
        ISearchRepository searchRepository,
        ICacheService cacheService,
        ILogger<SearchEngine> logger,
        SearchEngineConfig config)
    {
        _aiGateway = aiGateway;
        _vectorStore = vectorStore;
        _searchRepository = searchRepository;
        _cacheService = cacheService;
        _logger = logger;
        _config = config;
    }
    
    /// <summary>
    /// Main search endpoint with automatic type selection
    /// </summary>
    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        return request.SearchType switch
        {
            SearchType.Semantic => await SemanticSearchAsync(request, cancellationToken),
            SearchType.Keyword => await KeywordSearchAsync(request, cancellationToken),
            SearchType.Hybrid => await HybridSearchAsync(request, cancellationToken),
            SearchType.AI => await AISearchAsync(request, cancellationToken),
            _ => await HybridSearchAsync(request, cancellationToken)
        };
    }
    
    /// <summary>
    /// Pure semantic/vector search
    /// </summary>
    public async Task<SearchResponse> SemanticSearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogDebug("Performing semantic search: {Query}", request.Query);
        
        // Generate query embedding
        var embedResponse = await _aiGateway.EmbedAsync(new EmbedRequest
        {
            Texts = new List<string> { request.Query },
            UserId = request.UserId,
            TenantId = request.TenantId
        }, cancellationToken);
        
        var queryVector = embedResponse.Embeddings.FirstOrDefault()?.Vector ?? Array.Empty<float>();
        
        // Search vector store
        var vectorResults = await _vectorStore.SearchAsync(new VectorSearchRequest
        {
            QueryVector = queryVector,
            TopK = request.Limit,
            MinScore = request.MinRelevanceScore,
            FilterDocumentIds = request.FilterDocumentIds,
            MetadataFilters = request.MetadataFilters,
            UserId = request.UserId,
            TenantId = request.TenantId
        }, cancellationToken);
        
        stopwatch.Stop();
        
        var results = vectorResults.Results.Select(r => new SearchResult
        {
            DocumentId = r.DocumentId,
            ChunkId = r.ChunkId,
            Title = r.Metadata.GetValueOrDefault("title", "Untitled"),
            Content = r.Content,
            Url = r.Metadata.GetValueOrDefault("url"),
            Score = r.Score,
            SemanticScore = r.Score,
            KeywordScore = 0,
            Highlights = request.IncludeHighlights ? GenerateHighlights(r.Content, request.Query) : new(),
            Metadata = r.Metadata,
            PageNumber = int.TryParse(r.Metadata.GetValueOrDefault("page", "0"), out var page) ? page : 0
        }).ToList();
        
        return new SearchResponse
        {
            Results = results,
            Query = request.Query,
            SearchType = SearchType.Semantic,
            TotalResults = results.Count,
            RetrievedResults = results.Count,
            SearchTime = stopwatch.Elapsed
        };
    }
    
    /// <summary>
    /// Pure keyword/full-text search
    /// </summary>
    public async Task<SearchResponse> KeywordSearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogDebug("Performing keyword search: {Query}", request.Query);
        
        // Search using PostgreSQL full-text search
        var dbResults = await _searchRepository.SearchKeywordAsync(new KeywordSearchQuery
        {
            Query = request.Query,
            Limit = request.Limit,
            MinScore = request.MinRelevanceScore,
            FilterDocumentIds = request.FilterDocumentIds,
            MetadataFilters = request.MetadataFilters,
            UserId = request.UserId,
            TenantId = request.TenantId
        }, cancellationToken);
        
        stopwatch.Stop();
        
        var results = dbResults.Select(r => new SearchResult
        {
            DocumentId = r.DocumentId,
            ChunkId = r.ChunkId,
            Title = r.Title,
            Content = r.Content,
            Url = r.Url,
            Score = r.Rank,
            SemanticScore = 0,
            KeywordScore = r.Rank,
            Highlights = request.IncludeHighlights ? GenerateHighlights(r.Content, request.Query) : new(),
            Metadata = r.Metadata,
            PageNumber = r.PageNumber,
            CreatedAt = r.CreatedAt
        }).ToList();
        
        return new SearchResponse
        {
            Results = results,
            Query = request.Query,
            SearchType = SearchType.Keyword,
            TotalResults = results.Count,
            RetrievedResults = results.Count,
            SearchTime = stopwatch.Elapsed
        };
    }
    
    /// <summary>
    /// Hybrid search combining semantic + keyword
    /// </summary>
    public async Task<SearchResponse> HybridSearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogDebug("Performing hybrid search: {Query}", request.Query);
        
        // Run semantic and keyword searches in parallel
        var semanticTask = SemanticSearchAsync(new SearchRequest
        {
            Query = request.Query,
            UserId = request.UserId,
            TenantId = request.TenantId,
            Limit = request.Limit * 2, // Get more for merging
            MinRelevanceScore = request.MinRelevanceScore * 0.8, // Lower threshold
            FilterDocumentIds = request.FilterDocumentIds,
            MetadataFilters = request.MetadataFilters,
            IncludeHighlights = false
        }, cancellationToken);
        
        var keywordTask = KeywordSearchAsync(new SearchRequest
        {
            Query = request.Query,
            UserId = request.UserId,
            TenantId = request.TenantId,
            Limit = request.Limit * 2,
            MinRelevanceScore = request.MinRelevanceScore * 0.8,
            FilterDocumentIds = request.FilterDocumentIds,
            MetadataFilters = request.MetadataFilters,
            IncludeHighlights = false
        }, cancellationToken);
        
        await Task.WhenAll(semanticTask, keywordTask);
        
        var semanticResults = semanticTask.Result.Results;
        var keywordResults = keywordTask.Result.Results;
        
        // Merge and re-rank results using Reciprocal Rank Fusion (RRF)
        var mergedResults = MergeResultsWithRRF(
            semanticResults, 
            keywordResults,
            _config.VectorWeight,
            _config.KeywordWeight,
            request.Limit);
        
        // Add highlights
        if (request.IncludeHighlights)
        {
            foreach (var result in mergedResults)
            {
                result.Highlights = GenerateHighlights(result.Content, request.Query);
            }
        }
        
        stopwatch.Stop();
        
        // Generate facets if requested
        List<SearchFacet>? facets = null;
        if (request.IncludeFacets)
        {
            facets = GenerateFacets(mergedResults);
        }
        
        return new SearchResponse
        {
            Results = mergedResults,
            Query = request.Query,
            SearchType = SearchType.Hybrid,
            TotalResults = semanticResults.Count + keywordResults.Count,
            RetrievedResults = mergedResults.Count,
            SearchTime = stopwatch.Elapsed,
            Facets = facets,
            Analytics = new Dictionary<string, object>
            {
                ["semantic_count"] = semanticResults.Count,
                ["keyword_count"] = keywordResults.Count,
                ["vector_weight"] = _config.VectorWeight,
                ["keyword_weight"] = _config.KeywordWeight
            }
        };
    }
    
    /// <summary>
    /// AI-powered search with query understanding and reformulation
    /// </summary>
    public async Task<SearchResponse> AISearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogDebug("Performing AI search: {Query}", request.Query);
        
        // Step 1: Understand and reformulate query
        var queryAnalysis = await AnalyzeQueryAsync(request.Query, cancellationToken);
        
        // Step 2: Generate multiple query variations
        var queryVariations = await GenerateQueryVariationsAsync(request.Query, cancellationToken);
        
        // Step 3: Search with each variation and merge
        var allResults = new List<SearchResult>();
        
        var searches = queryVariations.Select(async variation =>
        {
            var searchResult = await HybridSearchAsync(new SearchRequest
            {
                Query = variation,
                UserId = request.UserId,
                TenantId = request.TenantId,
                Limit = request.Limit,
                MinRelevanceScore = request.MinRelevanceScore,
                FilterDocumentIds = request.FilterDocumentIds,
                MetadataFilters = request.MetadataFilters,
                IncludeHighlights = false
            }, cancellationToken);
            
            return searchResult.Results;
        });
        
        var resultsArrays = await Task.WhenAll(searches);
        
        foreach (var results in resultsArrays)
        {
            allResults.AddRange(results);
        }
        
        // Step 4: Deduplicate and re-rank
        var uniqueResults = DeduplicateAndMergeResults(allResults, request.Limit);
        
        // Step 5: Generate summary if requested
        stopwatch.Stop();
        
        // Add highlights
        if (request.IncludeHighlights)
        {
            foreach (var result in uniqueResults)
            {
                result.Highlights = GenerateHighlights(result.Content, request.Query);
            }
        }
        
        return new SearchResponse
        {
            Results = uniqueResults,
            Query = request.Query,
            SearchType = SearchType.AI,
            TotalResults = allResults.Count,
            RetrievedResults = uniqueResults.Count,
            SearchTime = stopwatch.Elapsed,
            Analytics = new Dictionary<string, object>
            {
                ["query_analysis"] = queryAnalysis,
                ["query_variations"] = queryVariations,
                ["variations_searched"] = queryVariations.Count
            }
        };
    }
    
    /// <summary>
    /// Autocomplete suggestions
    /// </summary>
    public async Task<List<string>> AutocompleteAsync(string prefix, int limit = 10, CancellationToken cancellationToken = default)
    {
        // Get recent searches and common terms
        var suggestions = await _searchRepository.GetAutocompleteSuggestionsAsync(prefix, limit, cancellationToken);
        return suggestions;
    }
    
    // ============================================================
    // HELPER METHODS
    // ============================================================
    
    /// <summary>
    /// Merge results using Reciprocal Rank Fusion (RRF)
    /// RRF = 1 / (k + rank), where k is a constant (usually 60)
    /// </summary>
    private List<SearchResult> MergeResultsWithRRF(
        List<SearchResult> semanticResults,
        List<SearchResult> keywordResults,
        double vectorWeight,
        double keywordWeight,
        int limit)
    {
        const int k = 60; // RRF constant
        var scoredResults = new Dictionary<string, (SearchResult Result, double Score)>();
        
        // Score semantic results
        for (int i = 0; i < semanticResults.Count; i++)
        {
            var result = semanticResults[i];
            var rrfScore = (1.0 / (k + i + 1)) * vectorWeight;
            var finalScore = result.SemanticScore * rrfScore * 100;
            
            result.Score = finalScore;
            scoredResults[result.ChunkId] = (result, finalScore);
        }
        
        // Score keyword results and merge
        for (int i = 0; i < keywordResults.Count; i++)
        {
            var result = keywordResults[i];
            var rrfScore = (1.0 / (k + i + 1)) * keywordWeight;
            var finalScore = result.KeywordScore * rrfScore * 100;
            
            if (scoredResults.TryGetValue(result.ChunkId, out var existing))
            {
                // Merge scores
                existing.Result.Score += finalScore;
                existing.Result.KeywordScore = result.KeywordScore;
                scoredResults[result.ChunkId] = (existing.Result, existing.Result.Score);
            }
            else
            {
                result.Score = finalScore;
                result.SemanticScore = 0;
                scoredResults[result.ChunkId] = (result, finalScore);
            }
        }
        
        return scoredResults.Values
            .OrderByDescending(x => x.Score)
            .Take(limit)
            .Select(x => x.Result)
            .ToList();
    }
    
    /// <summary>
    /// Deduplicate and merge AI search results
    /// </summary>
    private List<SearchResult> DeduplicateAndMergeResults(List<SearchResult> allResults, int limit)
    {
        var seen = new HashSet<string>();
        var uniqueResults = new List<SearchResult>();
        
        // Sort by score and deduplicate
        foreach (var result in allResults.OrderByDescending(r => r.Score))
        {
            if (seen.Add(result.ChunkId))
            {
                uniqueResults.Add(result);
                
                if (uniqueResults.Count >= limit)
                    break;
            }
        }
        
        return uniqueResults;
    }
    
    /// <summary>
    /// Generate text highlights
    /// </summary>
    private List<string> GenerateHighlights(string content, string query)
    {
        var highlights = new List<string>();
        var queryTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        var lowerContent = content.ToLower();
        
        foreach (var term in queryTerms)
        {
            var index = lowerContent.IndexOf(term, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                var start = Math.Max(0, index - 50);
                var end = Math.Min(content.Length, index + term.Length + 50);
                var highlight = content.Substring(start, end - start);
                
                if (start > 0) highlight = "..." + highlight;
                if (end < content.Length) highlight += "...";
                
                if (!highlights.Contains(highlight))
                    highlights.Add(highlight);
            }
        }
        
        return highlights.Take(3).ToList();
    }
    
    /// <summary>
    /// Generate facets from search results
    /// </summary>
    private List<SearchFacet> GenerateFacets(List<SearchResult> results)
    {
        var facets = new List<SearchFacet>();
        
        // Category facet
        var categories = results
            .Where(r => r.Metadata.TryGetValue("category", out var cat))
            .GroupBy(r => r.Metadata["category"])
            .Select(g => new FacetValue { Value = g.Key, Count = g.Count() })
            .OrderByDescending(f => f.Count)
            .ToList();
        
        if (categories.Any())
        {
            facets.Add(new SearchFacet { Name = "category", Values = categories });
        }
        
        return facets;
    }
    
    /// <summary>
    /// Analyze query using AI
    /// </summary>
    private async Task<QueryAnalysis> AnalyzeQueryAsync(string query, CancellationToken cancellationToken)
    {
        var chatResponse = await _aiGateway.ChatAsync(new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = "Analyze this search query. Return JSON with: intent (informational/navigational/transactional), entities (key entities), language, and suggested improvements." },
                new() { Role = MessageRole.User, Content = query }
            },
            Options = new AIRequestOptions { Temperature = 0.3, MaxTokens = 200 }
        }, cancellationToken);
        
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<QueryAnalysis>(chatResponse.Content) 
                ?? new QueryAnalysis { OriginalQuery = query };
        }
        catch
        {
            return new QueryAnalysis { OriginalQuery = query };
        }
    }
    
    /// <summary>
    /// Generate query variations for AI search
    /// </summary>
    private async Task<List<string>> GenerateQueryVariationsAsync(string query, CancellationToken cancellationToken)
    {
        var variations = new List<string> { query };
        
        var chatResponse = await _aiGateway.ChatAsync(new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = $"Generate 3 alternative search queries based on this query. Return as JSON array of strings. Queries should explore different aspects or rephrasing." },
                new() { Role = MessageRole.User, Content = query }
            },
            Options = new AIRequestOptions { Temperature = 0.7, MaxTokens = 300 }
        }, cancellationToken);
        
        try
        {
            var additional = System.Text.Json.JsonSerializer.Deserialize<List<string>>(chatResponse.Content);
            if (additional != null)
            {
                variations.AddRange(additional);
            }
        }
        catch
        {
            // Fallback: add simple variations
            variations.Add(query + " guide");
            variations.Add("how to " + query);
        }
        
        return variations.Take(4).ToList();
    }
}

/// <summary>
/// Query analysis result
/// </summary>
public class QueryAnalysis
{
    public string OriginalQuery { get; set; } = string.Empty;
    public string Intent { get; set; } = "informational";
    public List<string> Entities { get; set; } = new();
    public string Language { get; set; } = "en";
    public string? SuggestedImprovement { get; set; }
}

// ============================================================
// REPOSITORY INTERFACE
// ============================================================

public class KeywordSearchQuery
{
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 20;
    public double MinScore { get; set; } = 0.5;
    public List<string>? FilterDocumentIds { get; set; }
    public Dictionary<string, string>? MetadataFilters { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
}

public class KeywordSearchResult
{
    public string DocumentId { get; set; } = string.Empty;
    public string ChunkId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Url { get; set; }
    public double Rank { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
    public int PageNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public interface ISearchRepository
{
    Task<List<KeywordSearchResult>> SearchKeywordAsync(KeywordSearchQuery query, CancellationToken cancellationToken = default);
    Task<List<string>> GetAutocompleteSuggestionsAsync(string prefix, int limit, CancellationToken cancellationToken = default);
}

// ============================================================
// CONFIG
// ============================================================

public class SearchEngineConfig
{
    public bool EnableHybridSearch { get; set; } = true;
    public double VectorWeight { get; set; } = 0.7;
    public double KeywordWeight { get; set; } = 0.3;
    public bool EnableAIQueryUnderstanding { get; set; } = true;
    public int MaxQueryVariations { get; set; } = 4;
    public int HighlightContextLength { get; set; } = 100;
    public bool EnableFacets { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 30;
}
