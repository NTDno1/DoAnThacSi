// ChatMessage Entity
namespace AIBaseFramework.API.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SessionId { get; set; }
    public MessageRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Citations { get; set; } = "[]"; // JSON array
    public int? TokenCount { get; set; }
    public string? ModelUsed { get; set; }
    public int? LatencyMs { get; set; }
    public string SourcesUsed { get; set; } = "[]"; // JSON array
    public string TokenUsage { get; set; } = "{}"; // JSON object
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ChatSession? Session { get; set; }
}

public enum MessageRole
{
    System,
    User,
    Assistant
}
