// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: RAG Pipeline - Retrieval Augmented Generation
// ============================================================

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AIBaseFramework.AI.Gateway;
using AIBaseFramework.AI.Providers.Abstractions;

namespace AIBaseFramework.AI.RAG;

// ============================================================
// RAG CONTEXT
// ============================================================

/// <summary>
/// Retrieved document context for RAG
/// </summary>
public class RAGContext
{
    public string Content { get; set; } = string.Empty;
    public string SourceDocumentId { get; set; } = string.Empty;
    public string SourceTitle { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public int PageNumber { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

/// <summary>
/// RAG Request
/// </summary>
public class RAGRequest
{
    public string Query { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public int MaxChunks { get; set; } = 10;
    public double MinRelevanceScore { get; set; } = 0.5;
    public List<string>? FilterDocumentIds { get; set; }
    public Dictionary<string, string>? MetadataFilters { get; set; }
    public bool IncludeCitations { get; set; } = true;
    public string? SystemPromptOverride { get; set; }
}

/// <summary>
/// RAG Response with answer and citations
/// </summary>
public class RAGResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<RAGCitation> Citations { get; set; } = new();
    public List<RAGContext> RetrievedContexts { get; set; } = new();
    public string ModelUsed { get; set; } = string.Empty;
    public int TotalTokens { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public RAGMetrics Metrics { get; set; } = new();
}

/// <summary>
/// Citation for source reference
/// </summary>
public class RAGCitation
{
    public string CitationId { get; set; } = string.Empty;
    public string SourceDocumentId { get; set; } = string.Empty;
    public string SourceTitle { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public double Confidence { get; set; }
}

/// <summary>
/// RAG metrics
/// </summary>
public class RAGMetrics
{
    public int ChunksRetrieved { get; set; }
    public double AverageRelevanceScore { get; set; }
    public int TotalChunksInDatabase { get; set; }
    public TimeSpan RetrievalTime { get; set; }
    public TimeSpan GenerationTime { get; set; }
    public bool UsedCache { get; set; }
}

// ============================================================
// RAG PIPELINE
// ============================================================

/// <summary>
/// RAG Pipeline - Retrieval Augmented Generation
/// </summary>
public interface IRAGPipeline
{
    Task<RAGResponse> QueryAsync(RAGRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> QueryStreamAsync(RAGRequest request, CancellationToken cancellationToken = default);
    Task<List<RAGContext>> RetrieveContextAsync(RAGRequest request, CancellationToken cancellationToken = default);
    string BuildPrompt(string query, List<RAGContext> contexts, string? systemPrompt = null);
}

/// <summary>
/// RAG Pipeline implementation
/// </summary>
public class RAGPipeline : IRAGPipeline
{
    private readonly IAI Gateway _aiGateway;
    private readonly IVectorStore _vectorStore;
    private readonly ICacheService _cacheService;
    private readonly ILogger<RAGPipeline> _logger;
    private readonly RAGPipelineConfig _config;
    
    public RAGPipeline(
        IAI Gateway aiGateway,
        IVectorStore vectorStore,
        ICacheService cacheService,
        ILogger<RAGPipeline> logger,
        RAGPipelineConfig config)
    {
        _aiGateway = aiGateway;
        _vectorStore = vectorStore;
        _cacheService = cacheService;
        _logger = logger;
        _config = config;
    }
    
    /// <summary>
    /// Main RAG query - retrieve context and generate answer
    /// </summary>
    public async Task<RAGResponse> QueryAsync(RAGRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var metrics = new RAGMetrics();
        
        _logger.LogInformation("Starting RAG query: {Query}", request.Query.Substring(0, Math.Min(100, request.Query.Length)));
        
        try
        {
            // Step 1: Retrieve relevant context
            var retrievalStopwatch = Stopwatch.StartNew();
            var contexts = await RetrieveContextAsync(request, cancellationToken);
            retrievalStopwatch.Stop();
            
            metrics.ChunksRetrieved = contexts.Count;
            metrics.AverageRelevanceScore = contexts.Any() 
                ? contexts.Average(c => c.RelevanceScore) : 0;
            metrics.RetrievalTime = retrievalStopwatch.Elapsed;
            
            if (!contexts.Any())
            {
                _logger.LogWarning("No relevant context found for query");
                return new RAGResponse
                {
                    Answer = _config.NoContextResponse,
                    Metrics = metrics
                };
            }
            
            // Step 2: Build prompt
            var systemPrompt = request.SystemPromptOverride ?? _config.DefaultSystemPrompt;
            var prompt = BuildPrompt(request.Query, contexts, systemPrompt);
            
            // Step 3: Generate response
            var generationStopwatch = Stopwatch.StartNew();
            
            var chatRequest = new ChatRequest
            {
                Messages = new List<ChatMessage>
                {
                    new() { Role = MessageRole.System, Content = systemPrompt },
                    new() { Role = MessageRole.User, Content = prompt }
                },
                Options = new AIRequestOptions
                {
                    Temperature = _config.Temperature,
                    MaxTokens = _config.MaxTokens,
                    UserId = request.UserId,
                    TenantId = request.TenantId,
                    ModelId = _config.ModelId
                }
            };
            
            var chatResponse = await _aiGateway.ChatAsync(chatRequest, cancellationToken);
            
            generationStopwatch.Stop();
            metrics.GenerationTime = generationStopwatch.Elapsed;
            
            // Step 4: Build citations
            var citations = new List<RAGCitation>();
            if (request.IncludeCitations)
            {
                citations = BuildCitations(contexts, chatResponse.Content);
            }
            
            stopwatch.Stop();
            
            return new RAGResponse
            {
                Answer = chatResponse.Content,
                Citations = citations,
                RetrievedContexts = contexts,
                ModelUsed = chatResponse.Model,
                TotalTokens = chatResponse.TotalTokens,
                ProcessingTime = stopwatch.Elapsed,
                Metrics = metrics
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RAG query failed");
            throw;
        }
    }
    
    /// <summary>
    /// Stream RAG response for real-time output
    /// </summary>
    public async IAsyncEnumerable<string> QueryStreamAsync(
        RAGRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var contexts = await RetrieveContextAsync(request, cancellationToken);
        
        if (!contexts.Any())
        {
            yield return _config.NoContextResponse;
            yield break;
        }
        
        var systemPrompt = request.SystemPromptOverride ?? _config.DefaultSystemPrompt;
        var prompt = BuildPrompt(request.Query, contexts, systemPrompt);
        
        var chatRequest = new ChatRequest
        {
            Messages = new List<ChatMessage>
            {
                new() { Role = MessageRole.System, Content = systemPrompt },
                new() { Role = MessageRole.User, Content = prompt }
            },
            Options = new AIRequestOptions
            {
                Temperature = _config.Temperature,
                MaxTokens = _config.MaxTokens,
                UserId = request.UserId,
                TenantId = request.TenantId,
                ModelId = _config.ModelId
            }
        };
        
        await foreach (var delta in _aiGateway.StreamChatAsync(chatRequest, cancellationToken))
        {
            if (delta.Content != null)
            {
                yield return delta.Content;
            }
        }
    }
    
    /// <summary>
    /// Retrieve relevant context from vector store
    /// </summary>
    public async Task<List<RAGContext>> RetrieveContextAsync(RAGRequest request, CancellationToken cancellationToken = default)
    {
        // Check cache first
        var cacheKey = $"rag_context:{ComputeQueryHash(request.Query)}";
        var cachedContexts = await _cacheService.GetAsync<List<RAGContext>>(cacheKey);
        
        if (cachedContexts != null)
        {
            _logger.LogDebug("Using cached context for query");
            return cachedContexts;
        }
        
        // Generate query embedding
        var embedRequest = new EmbedRequest
        {
            Texts = new List<string> { request.Query },
            UserId = request.UserId,
            TenantId = request.TenantId
        };
        
        var embedResponse = await _aiGateway.EmbedAsync(embedRequest, cancellationToken);
        var queryVector = embedResponse.Embeddings.FirstOrDefault()?.Vector ?? Array.Empty<float>();
        
        // Search vector store
        var searchResult = await _vectorStore.SearchAsync(new VectorSearchRequest
        {
            QueryVector = queryVector,
            TopK = request.MaxChunks,
            MinScore = request.MinRelevanceScore,
            FilterDocumentIds = request.FilterDocumentIds,
            MetadataFilters = request.MetadataFilters,
            UserId = request.UserId,
            TenantId = request.TenantId
        }, cancellationToken);
        
        var contexts = searchResult.Results.Select(r => new RAGContext
        {
            Content = r.Content,
            SourceDocumentId = r.DocumentId,
            SourceTitle = r.Metadata.GetValueOrDefault("title", "Unknown"),
            SourceUrl = r.Metadata.GetValueOrDefault("url", ""),
            RelevanceScore = r.Score,
            PageNumber = int.TryParse(r.Metadata.GetValueOrDefault("page", "0"), out var page) ? page : 0,
            Metadata = r.Metadata
        }).ToList();
        
        // Cache contexts
        await _cacheService.SetAsync(cacheKey, contexts, TimeSpan.FromHours(_config.CacheDurationHours));
        
        return contexts;
    }
    
    /// <summary>
    /// Build RAG prompt with context
    /// </summary>
    public string BuildPrompt(string query, List<RAGContext> contexts, string? systemPrompt = null)
    {
        var contextSection = new System.Text.StringBuilder();
        contextSection.AppendLine("=== REFERENCE DOCUMENTS ===");
        contextSection.AppendLine();
        
        for (int i = 0; i < contexts.Count; i++)
        {
            var ctx = contexts[i];
            contextSection.AppendLine($"[Document {i + 1}]: {ctx.SourceTitle}");
            if (!string.IsNullOrEmpty(ctx.SourceUrl))
            {
                contextSection.AppendLine($"Source: {ctx.SourceUrl}");
            }
            if (ctx.PageNumber > 0)
            {
                contextSection.AppendLine($"Page: {ctx.PageNumber}");
            }
            contextSection.AppendLine($"Relevance: {ctx.RelevanceScore:P2}");
            contextSection.AppendLine();
            contextSection.AppendLine(ctx.Content);
            contextSection.AppendLine();
            contextSection.AppendLine(new string('-', 50));
            contextSection.AppendLine();
        }
        
        contextSection.AppendLine("=== USER QUESTION ===");
        contextSection.AppendLine(query);
        contextSection.AppendLine();
        contextSection.AppendLine("=== INSTRUCTIONS ===");
        contextSection.AppendLine("Based on the reference documents above, answer the user's question.");
        contextSection.AppendLine("If the documents don't contain enough information to answer the question, say so.");
        contextSection.AppendLine("Cite your sources using [Document N] format where N is the document number.");
        contextSection.AppendLine("Do not make up information that is not supported by the documents.");
        
        return contextSection.ToString();
    }
    
    /// <summary>
    /// Build citations from context and answer
    /// </summary>
    private List<RAGCitation> BuildCitations(List<RAGContext> contexts, string answer)
    {
        var citations = new List<RAGCitation>();
        
        for (int i = 0; i < contexts.Count; i++)
        {
            var ctx = contexts[i];
            var citationId = $"[Document {i + 1}]";
            
            // Find if document is cited in answer
            var index = answer.IndexOf(citationId, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                citations.Add(new RAGCitation
                {
                    CitationId = citationId,
                    SourceDocumentId = ctx.SourceDocumentId,
                    SourceTitle = ctx.SourceTitle,
                    Content = ctx.Content.Length > 200 
                        ? ctx.Content.Substring(0, 200) + "..." 
                        : ctx.Content,
                    StartIndex = index,
                    EndIndex = index + citationId.Length,
                    Confidence = ctx.RelevanceScore
                });
            }
        }
        
        return citations;
    }
    
    private static string ComputeQueryHash(string query)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(query.ToLowerInvariant().Trim());
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash).Substring(0, 16);
    }
}

// ============================================================
// VECTOR STORE INTERFACE
// ============================================================

public class VectorSearchRequest
{
    public float[] QueryVector { get; set; } = Array.Empty<float>();
    public int TopK { get; set; } = 10;
    public double MinScore { get; set; } = 0.5;
    public List<string>? FilterDocumentIds { get; set; }
    public Dictionary<string, string>? MetadataFilters { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
}

public class VectorSearchResult
{
    public string DocumentId { get; set; } = string.Empty;
    public string ChunkId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double Score { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class VectorSearchResponse
{
    public List<VectorSearchResult> Results { get; set; } = new();
    public TimeSpan SearchTime { get; set; }
    public int TotalResults { get; set; }
}

public interface IVectorStore
{
    Task<VectorSearchResponse> SearchAsync(VectorSearchRequest request, CancellationToken cancellationToken = default);
    Task<List<VectorSearchResult>> SearchHybridAsync(VectorSearchRequest request, string keywordQuery, CancellationToken cancellationToken = default);
    Task IndexDocumentAsync(DocumentIndexRequest request, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default);
}

public class DocumentIndexRequest
{
    public string DocumentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Url { get; set; }
    public List<DocumentChunk> Chunks { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class DocumentChunk
{
    public string Content { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public int ChunkIndex { get; set; }
    public int? PageNumber { get; set; }
}

// ============================================================
// CACHE INTERFACE
// ============================================================

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}

// ============================================================
// RAG CONFIG
// ============================================================

public class RAGPipelineConfig
{
    public string ModelId { get; set; } = "llama3.2:3b";
    public double Temperature { get; set; } = 0.3; // Lower for factual responses
    public int MaxTokens { get; set; } = 2048;
    public int MaxContextChunks { get; set; } = 10;
    public double MinRelevanceScore { get; set; } = 0.5;
    public int CacheDurationHours { get; set; } = 24;
    public bool EnableHybridSearch { get; set; } = true;
    public double VectorWeight { get; set; } = 0.7;
    public double KeywordWeight { get; set; } = 0.3;
    public string DefaultSystemPrompt { get; set; } = @"You are a helpful AI assistant that answers questions based on the provided reference documents.
Your role is to provide accurate, factual answers using only the information from the documents.
If you're unsure or the documents don't contain enough information, clearly state that.
Always cite your sources using [Document N] format.";
    public string NoContextResponse { get; set; } = "I couldn't find relevant information in the document database to answer your question. Please try rephrasing or contact support if you believe this information should be available.";
}
