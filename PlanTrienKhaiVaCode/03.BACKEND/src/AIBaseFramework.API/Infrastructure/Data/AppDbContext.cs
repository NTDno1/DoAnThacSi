// AppDbContext - Entity Framework Core with pgvector
using AIBaseFramework.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<SearchLog> SearchLogs => Set<SearchLog>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable pgvector extension
        HasPostgresExtension(modelBuilder, "vector");

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
            entity.Property(e => e.Department).HasColumnName("department").HasMaxLength(255);
            entity.Property(e => e.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.EmailVerifiedAt).HasColumnName("email_verified_at");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Role);
        });

        // Document configuration
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("documents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            entity.Property(e => e.OriginalFilename).HasColumnName("original_filename").HasMaxLength(500).IsRequired();
            entity.Property(e => e.StoragePath).HasColumnName("storage_path").HasMaxLength(1000).IsRequired();
            entity.Property(e => e.FileSize).HasColumnName("file_size");
            entity.Property(e => e.MimeType).HasColumnName("mime_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.FileHash).HasColumnName("file_hash").HasMaxLength(64).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.ExtractedText).HasColumnName("extracted_text");
            entity.Property(e => e.PageCount).HasColumnName("page_count");
            entity.Property(e => e.Language).HasColumnName("language").HasMaxLength(10);
            entity.Property(e => e.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
            entity.Property(e => e.IsPublic).HasColumnName("is_public");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.IndexedAt).HasColumnName("indexed_at");

            entity.HasOne(e => e.Uploader)
                .WithMany(u => u.Documents)
                .HasForeignKey(e => e.UploaderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Documents)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UploaderId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.FileHash);
        });

        // DocumentChunk configuration with pgvector
        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.ToTable("document_chunks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.ChunkIndex).HasColumnName("chunk_index");
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.ContentHash).HasColumnName("content_hash").HasMaxLength(64);
            entity.Property(e => e.Embedding).HasColumnName("embedding").HasColumnType("vector(768)");
            entity.Property(e => e.PageNumber).HasColumnName("page_number");
            entity.Property(e => e.SectionTitle).HasColumnName("section_title").HasMaxLength(500);
            entity.Property(e => e.TokenCount).HasColumnName("token_count");
            entity.Property(e => e.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Document)
                .WithMany(d => d.Chunks)
                .HasForeignKey(e => e.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.DocumentId);
            entity.HasIndex(e => e.ContentHash);
        });

        // ChatSession configuration
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.ToTable("chat_sessions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255);
            entity.Property(e => e.MessageCount).HasColumnName("message_count");
            entity.Property(e => e.LastMessagePreview).HasColumnName("last_message_preview");
            entity.Property(e => e.Model).HasColumnName("model").HasMaxLength(100);
            entity.Property(e => e.IsArchived).HasColumnName("is_archived");
            entity.Property(e => e.IsPinned).HasColumnName("is_pinned");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(e => e.User)
                .WithMany(u => u.ChatSessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
        });

        // ChatMessage configuration
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("chat_messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.Citations).HasColumnName("citations").HasColumnType("jsonb");
            entity.Property(e => e.TokenCount).HasColumnName("token_count");
            entity.Property(e => e.ModelUsed).HasColumnName("model_used").HasMaxLength(100);
            entity.Property(e => e.LatencyMs).HasColumnName("latency_ms");
            entity.Property(e => e.SourcesUsed).HasColumnName("sources_used").HasColumnType("jsonb");
            entity.Property(e => e.TokenUsage).HasColumnName("token_usage").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasOne(e => e.Session)
                .WithMany(s => s.Messages)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.SessionId);
        });
    }

    private static void HasPostgresExtension(ModelBuilder modelBuilder, string extension)
    {
        modelBuilder.HasPostgresExtension(extension);
    }
}

// Additional entities for DbContext
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedByUserId { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}

public class DocumentCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public string Color { get; set; } = "#3B82F6";
    public string Icon { get; set; } = "file-text";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DocumentCategory? Parent { get; set; }
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

public class SearchLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public Guid? SessionId { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public float[]? QueryEmbedding { get; set; }
    public int ResultsCount { get; set; }
    public int? LatencyMs { get; set; }
    public string SearchType { get; set; } = "hybrid";
    public string TopResults { get; set; } = "[]";
    public string UserFeedback { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string Details { get; set; } = "{}";
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SystemSetting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = "{}";
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
