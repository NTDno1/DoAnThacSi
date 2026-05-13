// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Infrastructure: Database Context & Vector Store
// ============================================================

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIBaseFramework.Infrastructure.Data;

// ============================================================
// ENTITIES
// ============================================================

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Role { get; set; } = "User";
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Tenant Tenant { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
}

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedBy { get; set; }
    public string? CreatedByIp { get; set; }
    
    public User User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;
}

public class Tenant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    public Guid? ParentTenantId { get; set; }
    
    [MaxLength(500)]
    public string? LogoUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    public TenantAIConfig? AIConfig { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

public class TenantAIConfig
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TenantId { get; set; }
    
    [MaxLength(50)]
    public string LLMProvider { get; set; } = "ollama";
    
    [MaxLength(100)]
    public string ChatModel { get; set; } = "llama3.2:3b";
    
    [MaxLength(100)]
    public string EmbeddingModel { get; set; } = "nomic-embed-text";
    
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 4096;
    
    public string? SystemPrompt { get; set; }
    
    public bool EnableRAG { get; set; } = true;
    public bool EnableVoice { get; set; } = false;
    public bool EnableAgent { get; set; } = false;
    public bool EnableSQLQuery { get; set; } = false;
    
    public List<string> AllowedTools { get; set; } = new();
    public List<string> BlockedKeywords { get; set; } = new();
    
    public int RequestsPerMinute { get; set; } = 60;
    public int TokensPerDay { get; set; } = 100000;
    
    public Tenant Tenant { get; set; } = null!;
}

public class Document
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string FileType { get; set; } = string.Empty;
    
    public long FileSize { get; set; }
    
    [MaxLength(100)]
    public string FileHash { get; set; } = string.Empty;
    
    public Guid CategoryId { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Processing";
    
    public Guid? OwnerId { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? IndexedAt { get; set; }
    public int ChunkCount { get; set; }
    
    public DocumentCategory Category { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public User? Owner { get; set; }
    public ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
}

public class DocumentCategory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;
    
    public Guid? ParentId { get; set; }
    
    public int SortOrder { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DocumentCategory? Parent { get; set; }
    public ICollection<DocumentCategory> Children { get; set; } = new List<DocumentCategory>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

public class DocumentChunk
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid DocumentId { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public int ChunkIndex { get; set; }
    
    [Column(TypeName = "vector(768)")]
    public float[] Embedding { get; set; } = Array.Empty<float>();
    
    public int? PageNumber { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Document Document { get; set; } = null!;
}

public class ChatSession
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(200)]
    public string Title { get; set; } = "New Chat";
    
    public Guid UserId { get; set; }
    
    public Guid TenantId { get; set; }
    
    public int MessageCount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    
    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid SessionId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = string.Empty; // user, assistant, system
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public string? Citations { get; set; } // JSON
    
    public string? Intent { get; set; }
    
    public string? ModelUsed { get; set; }
    
    public int? TokensUsed { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ChatSession Session { get; set; } = null!;
}

public class SearchLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string QueryText { get; set; } = string.Empty;
    
    [Column(TypeName = "vector(768)")]
    public float[]? QueryEmbedding { get; set; }
    
    [MaxLength(50)]
    public string SearchType { get; set; } = "hybrid";
    
    public int ResultsCount { get; set; }
    
    public double SearchTimeMs { get; set; }
    
    public Guid? UserId { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;
    
    public Guid? EntityId { get; set; }
    
    public string? Details { get; set; } // JSON
    
    public Guid? UserId { get; set; }
    
    public string? UserEmail { get; set; }
    
    public string? IpAddress { get; set; }
    
    public Guid TenantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================
// DATABASE CONTEXT
// ============================================================

public class AIDbContext : DbContext
{
    public AIDbContext(DbContextOptions<AIDbContext> options) : base(options) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantAIConfig> TenantAIConfigs => Set<TenantAIConfig>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<SearchLog> SearchLogs => Set<SearchLog>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Vector extension
        modelBuilder.HasPostgresExtension("vector");
        
        // User indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        // Document indexes
        modelBuilder.Entity<Document>()
            .HasIndex(d => d.TenantId);
        modelBuilder.Entity<Document>()
            .HasIndex(d => d.CategoryId);
        modelBuilder.Entity<Document>()
            .HasIndex(d => d.Status);
        
        // DocumentChunk indexes
        modelBuilder.Entity<DocumentChunk>()
            .HasIndex(c => c.DocumentId);
        modelBuilder.Entity<DocumentChunk>()
            .HasIndex(c => c.TenantId);
        
        // Chat indexes
        modelBuilder.Entity<ChatSession>()
            .HasIndex(s => s.UserId);
        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.SessionId);
        
        // SearchLog index
        modelBuilder.Entity<SearchLog>()
            .HasIndex(s => s.TenantId);
        modelBuilder.Entity<SearchLog>()
            .HasIndex(s => s.CreatedAt);
        
        // AuditLog indexes
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.TenantId);
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.CreatedAt);
        
        // Tenant indexes
        modelBuilder.Entity<Tenant>()
            .HasIndex(t => t.Code)
            .IsUnique();
    }
}
