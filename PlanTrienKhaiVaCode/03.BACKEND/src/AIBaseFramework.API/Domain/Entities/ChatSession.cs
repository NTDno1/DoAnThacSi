// ChatSession Entity
namespace AIBaseFramework.API.Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = "New Chat";
    public int MessageCount { get; set; } = 0;
    public string? LastMessagePreview { get; set; }
    public string? Model { get; set; }
    public bool IsArchived { get; set; } = false;
    public bool IsPinned { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
