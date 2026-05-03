// DocumentChunk Entity - Vector Embeddings
namespace AIBaseFramework.API.Domain.Entities;

public class DocumentChunk
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>(); // 768 dimensions (Ollama nomic-embed-text)
    public int? PageNumber { get; set; }
    public string? SectionTitle { get; set; }
    public int? StartCharPosition { get; set; }
    public int? EndCharPosition { get; set; }
    public int? TokenCount { get; set; }
    public string Metadata { get; set; } = "{}";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Document? Document { get; set; }
}
