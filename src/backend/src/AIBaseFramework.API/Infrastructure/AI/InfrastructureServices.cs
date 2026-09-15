// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Infrastructure: Vector Store & Cache Service
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace AIBaseFramework.Infrastructure.AI;

// ============================================================
// VECTOR STORE (PostgreSQL + pgvector)
// ============================================================

public class PostgreSQLVectorStore : IVectorStore
{
    private readonly IDbContextFactory<AIDbContext> _dbContextFactory;
    private readonly ILogger<PostgreSQLVectorStore> _logger;
    
    public PostgreSQLVectorStore(
        IDbContextFactory<AIDbContext> dbContextFactory,
        ILogger<PostgreSQLVectorStore> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }
    
    public async Task<VectorSearchResponse> SearchAsync(
        VectorSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        // Build query with tenant filter
        var query = context.DocumentChunks
            .Include(c => c.Document)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(request.TenantId))
        {
            var tenantId = Guid.Parse(request.TenantId);
            query = query.Where(c => c.TenantId == tenantId);
        }
        
        if (request.FilterDocumentIds?.Any() == true)
        {
            var docIds = request.FilterDocumentIds.Select(Guid.Parse).ToList();
            query = query.Where(c => docIds.Contains(c.DocumentId));
        }
        
        // For pgvector similarity search, we use raw SQL
        var sql = @"
            SELECT 
                dc.""Id"" as ChunkId,
                dc.""DocumentId"" as DocumentId,
                dc.""Content"" as Content,
                dc.""ChunkIndex"",
                d.""Title"",
                d.""FilePath"",
                1 - (dc.""Embedding"" <=> @vector::vector) as similarity,
                dc.""Content"" as Metadata
            FROM ""DocumentChunks"" dc
            INNER JOIN ""Documents"" d ON dc.""DocumentId"" = d.""Id""
            WHERE dc.""TenantId"" = @tenantId
            AND 1 - (dc.""Embedding"" <=> @vector::vector) > @minScore
            ORDER BY dc.""Embedding"" <=> @vector::vector
            LIMIT @limit";
        
        var vectorString = "[" + string.Join(",", request.QueryVector.Select(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture))) + "]";
        var tenantIdGuid = string.IsNullOrEmpty(request.TenantId) ? Guid.Empty : Guid.Parse(request.TenantId);
        
        var results = await context.Database
            .SqlQueryRaw<VectorSearchRawResult>(sql, 
                new Npgsql.NpgsqlParameter("@vector", vectorString),
                new Npgsql.NpgsqlParameter("@tenantId", tenantIdGuid),
                new Npgsql.NpgsqlParameter("@minScore", request.MinScore),
                new Npgsql.NpgsqlParameter("@limit", request.TopK))
            .ToListAsync(cancellationToken);
        
        stopwatch.Stop();
        
        return new VectorSearchResponse
        {
            Results = results.Select(r => new VectorSearchResult
            {
                ChunkId = r.ChunkId.ToString(),
                DocumentId = r.DocumentId.ToString(),
                Content = r.Content,
                Score = r.Similarity,
                Metadata = new Dictionary<string, string>
                {
                    ["title"] = r.Title ?? "Untitled",
                    ["url"] = r.FilePath ?? ""
                }
            }).ToList(),
            SearchTime = stopwatch.Elapsed,
            TotalResults = results.Count
        };
    }
    
    public async Task<List<VectorSearchResult>> SearchHybridAsync(
        VectorSearchRequest request, 
        string keywordQuery, 
        CancellationToken cancellationToken = default)
    {
        // First do vector search
        var vectorResults = await SearchAsync(request, cancellationToken);
        
        // Then do keyword search
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var tenantId = string.IsNullOrEmpty(request.TenantId) ? Guid.Empty : Guid.Parse(request.TenantId);
        
        var keywordResults = await context.DocumentChunks
            .Include(c => c.Document)
            .Where(c => c.TenantId == tenantId)
            .Where(c => c.Content.Contains(keywordQuery))
            .Take(request.TopK)
            .Select(c => new VectorSearchResult
            {
                ChunkId = c.Id.ToString(),
                DocumentId = c.DocumentId.ToString(),
                Content = c.Content,
                Score = 0.5, // Will be recalculated
                Metadata = new Dictionary<string, string>
                {
                    ["title"] = c.Document.Title,
                    ["url"] = c.Document.FilePath
                }
            })
            .ToListAsync(cancellationToken);
        
        // Merge results (simple approach)
        var allResults = vectorResults.Results.Concat(keywordResults)
            .GroupBy(r => r.ChunkId)
            .Select(g => g.First())
            .Take(request.TopK)
            .ToList();
        
        return allResults;
    }
    
    public async Task IndexDocumentAsync(
        DocumentIndexRequest request, 
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        foreach (var chunk in request.Chunks)
        {
            var entity = new DocumentChunk
            {
                Id = Guid.NewGuid(),
                DocumentId = Guid.Parse(request.DocumentId),
                Content = chunk.Content,
                ChunkIndex = chunk.ChunkIndex,
                Embedding = chunk.Embedding,
                PageNumber = chunk.PageNumber,
                TenantId = Guid.Empty, // Will be set from document
                CreatedAt = DateTime.UtcNow
            };
            
            context.DocumentChunks.Add(entity);
        }
        
        await context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Indexed document {DocumentId} with {ChunkCount} chunks", 
            request.DocumentId, request.Chunks.Count);
    }
    
    public async Task DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var docId = Guid.Parse(documentId);
        var chunks = await context.DocumentChunks
            .Where(c => c.DocumentId == docId)
            .ToListAsync(cancellationToken);
        
        context.DocumentChunks.RemoveRange(chunks);
        await context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Deleted {Count} chunks for document {DocumentId}", chunks.Count, documentId);
    }
    
    private class VectorSearchRawResult
    {
        public Guid ChunkId { get; set; }
        public Guid DocumentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public string? Title { get; set; }
        public string? FilePath { get; set; }
        public double Similarity { get; set; }
    }
}

// ============================================================
// CACHE SERVICE (Redis)
// ============================================================

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ILogger<RedisCacheService> _logger;
    
    public RedisCacheService(
        IConnectionMultiplexer redis, 
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _logger = logger;
    }
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            
            if (value.IsNullOrEmpty)
            {
                return null;
            }
            
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache get failed for key: {Key}", key);
            return null;
        }
    }
    
    public async Task SetAsync<T>(
        string key, 
        T value, 
        TimeSpan? expiry = null, 
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache set failed for key: {Key}", key);
        }
    }
    
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache remove failed for key: {Key}", key);
        }
    }
    
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _db.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache exists check failed for key: {Key}", key);
            return false;
        }
    }
}

// ============================================================
// SEARCH REPOSITORY
// ============================================================

public class PostgreSQLSearchRepository : ISearchRepository
{
    private readonly IDbContextFactory<AIDbContext> _dbContextFactory;
    private readonly ILogger<PostgreSQLSearchRepository> _logger;
    
    public PostgreSQLSearchRepository(
        IDbContextFactory<AIDbContext> dbContextFactory,
        ILogger<PostgreSQLSearchRepository> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }
    
    public async Task<List<KeywordSearchResult>> SearchKeywordAsync(
        KeywordSearchQuery query, 
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var tenantId = string.IsNullOrEmpty(query.TenantId) ? Guid.Empty : Guid.Parse(query.TenantId);
        
        var results = await context.DocumentChunks
            .Include(c => c.Document)
            .Where(c => c.TenantId == tenantId)
            .Where(c => c.Content.ToLower().Contains(query.Query.ToLower()))
            .OrderByDescending(c => c.CreatedAt)
            .Take(query.Limit)
            .Select(c => new KeywordSearchResult
            {
                DocumentId = c.DocumentId.ToString(),
                ChunkId = c.Id.ToString(),
                Title = c.Document.Title,
                Content = c.Content,
                Url = c.Document.FilePath,
                Rank = 1.0,
                PageNumber = c.PageNumber ?? 0,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);
        
        return results;
    }
    
    public async Task<List<string>> GetAutocompleteSuggestionsAsync(
        string prefix, 
        int limit, 
        CancellationToken cancellationToken = default)
    {
        // Get recent search terms from logs
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var suggestions = await context.SearchLogs
            .Where(s => s.QueryText.ToLower().StartsWith(prefix.ToLower()))
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .Select(s => s.QueryText)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        return suggestions;
    }
}

// ============================================================
// SESSION STORE (Redis)
// ============================================================

public class RedisSessionStore : ISessionStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ILogger<RedisSessionStore> _logger;
    private readonly TimeSpan _sessionExpiry = TimeSpan.FromHours(24);
    
    public RedisSessionStore(IConnectionMultiplexer redis, ILogger<RedisSessionStore> logger)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _logger = logger;
    }
    
    public async Task<AgentSession?> GetAsync(string sessionId)
    {
        try
        {
            var key = $"agent:session:{sessionId}";
            var value = await _db.StringGetAsync(key);
            
            if (value.IsNullOrEmpty)
            {
                return null;
            }
            
            return JsonSerializer.Deserialize<AgentSession>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get session: {SessionId}", sessionId);
            return null;
        }
    }
    
    public async Task SaveAsync(AgentSession session)
    {
        try
        {
            var key = $"agent:session:{session.SessionId}";
            var json = JsonSerializer.Serialize(session);
            await _db.StringSetAsync(key, json, _sessionExpiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save session: {SessionId}", session.SessionId);
        }
    }
    
    public async Task DeleteAsync(string sessionId)
    {
        try
        {
            var key = $"agent:session:{sessionId}";
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete session: {SessionId}", sessionId);
        }
    }
    
    public async Task<List<AgentSession>> GetUserSessionsAsync(string userId)
    {
        // In production, use Redis SCAN to find sessions by user
        return new List<AgentSession>();
    }
}

// ============================================================
// AGENT MEMORY SERVICE
// ============================================================

public class AgentMemoryService : IAgentMemoryService
{
    private readonly ICacheService _cacheService;
    private readonly IAI.Gateway.IAI Gateway _aiGateway;
    private readonly ILogger<AgentMemoryService> _logger;
    
    public AgentMemoryService(
        ICacheService cacheService,
        IAI.Gateway.IAI Gateway aiGateway,
        ILogger<AgentMemoryService> logger)
    {
        _cacheService = cacheService;
        _aiGateway = aiGateway;
        _logger = logger;
    }
    
    public async Task AddMemoryAsync(MemoryItem memory, CancellationToken cancellationToken = default)
    {
        var key = $"memory:{memory.SessionId}";
        var memories = await _cacheService.GetAsync<List<MemoryItem>>(key, cancellationToken) ?? new();
        
        memories.Add(memory);
        
        // Keep only last 100 memories
        if (memories.Count > 100)
        {
            memories = memories.TakeLast(100).ToList();
        }
        
        await _cacheService.SetAsync(key, memories, TimeSpan.FromDays(7), cancellationToken);
    }
    
    public async Task<List<MemoryItem>> GetRelevantMemoriesAsync(
        string sessionId, 
        string query, 
        int limit, 
        CancellationToken cancellationToken = default)
    {
        var key = $"memory:{sessionId}";
        var memories = await _cacheService.GetAsync<List<MemoryItem>>(key, cancellationToken);
        
        if (memories == null || !memories.Any())
        {
            return new List<MemoryItem>();
        }
        
        // Simple keyword matching for relevance
        var queryLower = query.ToLower();
        var relevant = memories
            .Where(m => m.Content.ToLower().Contains(queryLower) || 
                        queryLower.Split(' ').Any(term => m.Content.ToLower().Contains(term)))
            .Take(limit)
            .ToList();
        
        return relevant.Any() ? relevant : memories.TakeLast(limit).ToList();
    }
    
    public async Task<List<MemoryItem>> GetRecentMemoriesAsync(
        string sessionId, 
        int limit, 
        CancellationToken cancellationToken = default)
    {
        var key = $"memory:{sessionId}";
        var memories = await _cacheService.GetAsync<List<MemoryItem>>(key, cancellationToken);
        
        return memories?.TakeLast(limit).ToList() ?? new List<MemoryItem>();
    }
    
    public async Task ClearSessionMemoriesAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var key = $"memory:{sessionId}";
        await _cacheService.RemoveAsync(key, cancellationToken);
    }
}
