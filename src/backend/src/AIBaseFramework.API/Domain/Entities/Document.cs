// Document Entity
using AIBaseFramework.API.Domain.Entities;

namespace AIBaseFramework.API.Domain.Entities;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string OriginalFilename { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public Guid UploaderId { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public string? ExtractedText { get; set; }
    public int? PageCount { get; set; }
    public string Language { get; set; } = "vi";
    public string Metadata { get; set; } = "{}";
    public bool IsPublic { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? IndexedAt { get; set; }

    // Navigation properties
    public User? Uploader { get; set; }
    public DocumentCategory? Category { get; set; }
    public ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
}

public enum DocumentStatus
{
    Pending,
    Processing,
    Indexed,
    Failed,
    Deleted
}
