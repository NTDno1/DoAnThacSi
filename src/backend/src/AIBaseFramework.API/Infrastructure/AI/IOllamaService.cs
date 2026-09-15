// Service Interfaces
namespace AIBaseFramework.API.Infrastructure.AI;

public interface IOllamaService
{
    Task<string> ChatAsync(string prompt, string? systemPrompt = null);
    Task<float[]> EmbedAsync(string text);
    Task<List<string>> ListModelsAsync();
}

public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
    Task<List<float[]>> GenerateBatchEmbeddingsAsync(List<string> texts);
    int EmbeddingDimensions { get; }
}

public interface IRAGPipeline
{
    Task<RAGResponse> GenerateResponseAsync(
        string userMessage,
        Guid sessionId,
        string? conversationHistory = null);
}

public class RAGResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<RAGSource> Sources { get; set; } = new();
    public float Confidence { get; set; }
    public int TokensUsed { get; set; }
    public int LatencyMs { get; set; }
}

public class RAGSource
{
    public Guid DocumentId { get; set; }
    public string DocumentTitle { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public string Excerpt { get; set; } = string.Empty;
    public float RelevanceScore { get; set; }
}
