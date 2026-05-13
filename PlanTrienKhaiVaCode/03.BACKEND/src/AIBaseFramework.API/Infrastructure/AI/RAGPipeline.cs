// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// RAG Pipeline Service - Retrieval Augmented Generation
// ============================================================

using AIBaseFramework.API.Domain.Entities;
using AIBaseFramework.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIBaseFramework.API.Infrastructure.AI;

public class RAGPipeline : IRAGPipeline
{
    private readonly IOllamaService _ollamaService;
    private readonly IEmbeddingService _embeddingService;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<RAGPipeline> _logger;

    private const string SystemPrompt = @"Bạn là một trợ lý AI chuyên trả lời câu hỏi dựa trên tài liệu được cung cấp.

QUY TẮC NGHIÊM NGẶT:
1. CHỈ sử dụng thông tin từ ngữ cảnh (context) được cung cấp bên dưới
2. NẾU không tìm thấy thông tin liên quan, hãy nói rõ ràng: 'Tôi không tìm thấy thông tin này trong tài liệu được cung cấp.'
3. KHÔNG được bịa đặt hoặc suy luận thông tin không có trong ngữ cảnh
4. LUÔN trích dẫn nguồn bằng cách ghi [1], [2], [3] tương ứng với tài liệu
5. Nếu có nhiều nguồn, hãy kết hợp thông tin từ các nguồn đó

Định dạng trả lời:
- Câu trả lời: [nội dung trả lời dựa trên ngữ cảnh]
- Nguồn: [trích dẫn tài liệu với số]";

    public RAGPipeline(
        IOllamaService ollamaService,
        IEmbeddingService embeddingService,
        AppDbContext dbContext,
        ILogger<RAGPipeline> logger)
    {
        _ollamaService = ollamaService;
        _embeddingService = embeddingService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<RAGResponse> GenerateResponseAsync(
        string userMessage,
        Guid sessionId,
        string? conversationHistory = null)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Step 1: Generate query embedding
            var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(userMessage);

            // Step 2: Retrieve relevant chunks
            var retrievedChunks = await RetrieveRelevantChunksAsync(queryEmbedding, topK: 5);

            if (!retrievedChunks.Any())
            {
                return new RAGResponse
                {
                    Answer = "Tôi không tìm thấy thông tin liên quan trong tài liệu.",
                    Confidence = 0,
                    Sources = new List<RAGSource>()
                };
            }

            // Step 3: Build context from chunks
            var context = BuildContext(retrievedChunks);

            // Step 4: Build RAG prompt
            var prompt = BuildPrompt(userMessage, context, conversationHistory);

            // Step 5: Generate response
            var response = await _ollamaService.ChatAsync(prompt, SystemPrompt);

            stopwatch.Stop();

            // Step 6: Extract sources
            var sources = retrievedChunks.Select(c => new RAGSource
            {
                DocumentId = c.DocumentId,
                DocumentTitle = c.Document?.Title ?? "Unknown",
                PageNumber = c.PageNumber ?? 0,
                Excerpt = c.Content.Length > 200 ? c.Content[..200] + "..." : c.Content,
                RelevanceScore = c.Similarity ?? 0
            }).ToList();

            return new RAGResponse
            {
                Answer = response,
                Sources = sources,
                Confidence = retrievedChunks.Average(c => c.Similarity ?? 0),
                TokensUsed = EstimateTokens(response),
                LatencyMs = (int)stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RAG pipeline failed for session: {SessionId}", sessionId);
            throw;
        }
    }

    private async Task<List<RetrievedChunk>> RetrieveRelevantChunksAsync(
        float[] queryEmbedding, 
        int topK)
    {
        // Use pgvector for similarity search
        var chunks = await _dbContext.DocumentChunks
            .Include(c => c.Document)
            .Where(c => c.IsActive && c.Document!.Status == DocumentStatus.Indexed)
            .OrderByDescending(c => 1 - c.Embedding.CosineDistanceLessThan(queryEmbedding))
            .Take(topK)
            .Select(c => new RetrievedChunk
            {
                Id = c.Id,
                DocumentId = c.DocumentId,
                Content = c.Content,
                PageNumber = c.PageNumber,
                Similarity = 1 - c.Embedding.CosineDistanceLessThan(queryEmbedding),
                Document = c.Document!
            })
            .ToListAsync();

        return chunks;
    }

    private static string BuildContext(List<RetrievedChunk> chunks)
    {
        var context = new System.Text.StringBuilder();
        context.AppendLine("NGỮ CẢNH TỪ TÀI LIỆU:");
        context.AppendLine();

        for (int i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            context.AppendLine($"[{i + 1}] {chunk.Document.Title}" +
                (chunk.PageNumber.HasValue ? $" (Trang {chunk.PageNumber})" : "") + ":");
            context.AppendLine(chunk.Content);
            context.AppendLine();
        }

        return context.ToString();
    }

    private static string BuildPrompt(
        string userMessage, 
        string context, 
        string? conversationHistory)
    {
        var prompt = new System.Text.StringBuilder();
        
        if (!string.IsNullOrEmpty(conversationHistory))
        {
            prompt.AppendLine("LỊCH SỬ CUỘC HỘI THOẠT:");
            prompt.AppendLine(conversationHistory);
            prompt.AppendLine();
        }

        prompt.AppendLine(context);
        prompt.AppendLine("CÂU HỎI CỦA NGƯỜI DÙNG:");
        prompt.AppendLine(userMessage);

        return prompt.ToString();
    }

    private static int EstimateTokens(string text)
    {
        // Rough estimate: ~4 characters per token
        return text.Length / 4;
    }
}

public class RetrievedChunk
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? PageNumber { get; set; }
    public float? Similarity { get; set; }
    public Document Document { get; set; } = null!;
}
