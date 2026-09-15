// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Document Service Implementation
// ============================================================

using System.IO;
using System.Security.Cryptography;
using System.Text;
using AIBaseFramework.API.Domain.Entities;
using AIBaseFramework.API.Infrastructure.AI;
using AIBaseFramework.API.Infrastructure.Storage;
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIBaseFramework.API.Services.Implementations;

public class DocumentService : IDocumentService
{
    private readonly AppDbContext _dbContext;
    private readonly IMinIOService _minioService;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<DocumentService> _logger;
    private readonly IConfiguration _configuration;

    private const long MaxFileSize = 50 * 1024 * 1024; // 50MB
    private static readonly string[] AllowedExtensions = { ".pdf", ".docx", ".doc", ".txt", ".md", ".xlsx", ".xls" };

    public DocumentService(
        AppDbContext dbContext,
        IMinIOService minioService,
        IEmbeddingService embeddingService,
        ILogger<DocumentService> logger,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _minioService = minioService;
        _embeddingService = embeddingService;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<DocumentUploadResult> UploadDocumentAsync(IFormFile file, Guid userId, Guid? categoryId = null)
    {
        // Validate file
        if (file == null || file.Length == 0)
        {
            throw new Exception("File không được để trống");
        }

        if (file.Length > MaxFileSize)
        {
            throw new Exception($"File vượt quá kích thước cho phép ({MaxFileSize / 1024 / 1024}MB)");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new Exception($"Định dạng file không được hỗ trợ: {extension}");
        }

        // Get user
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception("Không tìm thấy người dùng");
        }

        // Calculate file hash for deduplication
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        var fileHash = ComputeSHA256(fileBytes);

        // Check for duplicate
        var existingDoc = await _dbContext.Documents
            .FirstOrDefaultAsync(d => d.FileHash == fileHash);

        if (existingDoc != null)
        {
            throw new Exception("File đã tồn tại trong hệ thống");
        }

        // Generate storage path
        var objectName = $"{userId}/{Guid.NewGuid()}{extension}";

        // Upload to MinIO
        await _minioService.UploadFileAsync(new MemoryStream(fileBytes), file.FileName, GetMimeType(extension));

        // Create document record
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            OriginalFilename = file.FileName,
            StoragePath = objectName,
            FileSize = file.Length,
            MimeType = GetMimeType(extension),
            FileHash = fileHash,
            CategoryId = categoryId ?? Guid.Empty,
            UploaderId = userId,
            Status = DocumentStatus.Processing,
            Language = "vi",
            Metadata = "{}",
            IsPublic = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Documents.Add(document);
        await _dbContext.SaveChangesAsync();

        // Start background processing
        _ = Task.Run(async () =>
        {
            try
            {
                await ProcessDocumentAsync(document.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background document processing failed for {DocumentId}", document.Id);
            }
        });

        return new DocumentUploadResult(document.Id, document.Title, document.Status.ToString(), document.CreatedAt);
    }

    public async Task<PagedResult<DocumentDto>> GetDocumentsAsync(
        Guid userId, 
        int page, 
        int pageSize, 
        Guid? categoryId = null, 
        string? status = null)
    {
        var query = _dbContext.Documents
            .Include(d => d.Category)
            .Where(d => d.UploaderId == userId)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(d => d.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<DocumentStatus>(status, true, out var statusEnum))
        {
            query = query.Where(d => d.Status == statusEnum);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var documents = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DocumentDto(
                d.Id,
                d.Title,
                d.OriginalFilename,
                d.FileSize,
                d.MimeType,
                d.Status.ToString(),
                d.CreatedAt,
                d.CategoryId,
                d.Category != null ? d.Category.Name : null))
            .ToListAsync();

        return new PagedResult<DocumentDto>(documents, totalCount, page, pageSize, totalPages);
    }

    public async Task<DocumentDto?> GetDocumentByIdAsync(Guid id, Guid userId)
    {
        var document = await _dbContext.Documents
            .Include(d => d.Category)
            .FirstOrDefaultAsync(d => d.Id == id && d.UploaderId == userId);

        if (document == null) return null;

        return new DocumentDto(
            document.Id,
            document.Title,
            document.OriginalFilename,
            document.FileSize,
            document.MimeType,
            document.Status.ToString(),
            document.CreatedAt,
            document.CategoryId,
            document.Category?.Name);
    }

    public async Task DeleteDocumentAsync(Guid id, Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Không tìm thấy người dùng");
        }

        var document = await _dbContext.Documents
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
        {
            throw new Exception("Không tìm thấy tài liệu");
        }

        // Only owner or admin can delete
        if (document.UploaderId != userId && user.Role != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("Không có quyền xóa tài liệu này");
        }

        // Delete from MinIO
        try
        {
            await _minioService.DeleteFileAsync(document.StoragePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file from storage: {Path}", document.StoragePath);
        }

        // Delete chunks
        var chunks = await _dbContext.DocumentChunks
            .Where(c => c.DocumentId == id)
            .ToListAsync();
        _dbContext.DocumentChunks.RemoveRange(chunks);

        // Mark document as deleted
        document.Status = DocumentStatus.Deleted;
        document.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Document deleted: {Id}", id);
    }

    public async Task<string> GetDocumentStatusAsync(Guid id)
    {
        var document = await _dbContext.Documents.FindAsync(id);
        return document?.Status.ToString() ?? "Unknown";
    }

    public async Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(Guid id, Guid userId)
    {
        var document = await _dbContext.Documents
            .FirstOrDefaultAsync(d => d.Id == id && d.UploaderId == userId);

        if (document == null)
        {
            throw new Exception("Không tìm thấy tài liệu");
        }

        var fileBytes = await _minioService.DownloadFileAsync(document.StoragePath);
        var stream = new MemoryStream(fileBytes);

        return (stream, document.OriginalFilename, document.MimeType);
    }

    // Background processing
    private async Task ProcessDocumentAsync(Guid documentId)
    {
        var document = await _dbContext.Documents.FindAsync(documentId);
        if (document == null) return;

        try
        {
            document.Status = DocumentStatus.Processing;
            await _dbContext.SaveChangesAsync();

            // Download file
            var fileBytes = await _minioService.DownloadFileAsync(document.StoragePath);
            var content = ExtractText(fileBytes, document.MimeType);

            if (string.IsNullOrWhiteSpace(content))
            {
                document.Status = DocumentStatus.Failed;
                await _dbContext.SaveChangesAsync();
                return;
            }

            // Chunk text
            var chunks = ChunkText(content, 512, 50);

            // Generate embeddings and store
            foreach (var chunkContent in chunks)
            {
                var embedding = await _embeddingService.GenerateEmbeddingAsync(chunkContent);
                
                var chunk = new DocumentChunk
                {
                    Id = Guid.NewGuid(),
                    DocumentId = documentId,
                    ChunkIndex = chunks.IndexOf(chunkContent),
                    Content = chunkContent,
                    ContentHash = ComputeSHA256(Encoding.UTF8.GetBytes(chunkContent)),
                    Embedding = embedding,
                    Metadata = "{}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.DocumentChunks.Add(chunk);
            }

            document.Status = DocumentStatus.Indexed;
            document.PageCount = chunks.Count;
            document.IndexedAt = DateTime.UtcNow;
            document.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Document processed: {Id}, {ChunkCount} chunks", documentId, chunks.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document processing failed: {Id}", documentId);
            document.Status = DocumentStatus.Failed;
            await _dbContext.SaveChangesAsync();
        }
    }

    private List<string> ChunkText(string text, int chunkSize, int overlap)
    {
        var chunks = new List<string>();
        var sentences = text.Split(new[] { ". ", ".\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        var currentChunk = new StringBuilder();
        foreach (var sentence in sentences)
        {
            if (currentChunk.Length + sentence.Length > chunkSize && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString().Trim());
                var overlapText = currentChunk.ToString();
                currentChunk.Clear();
                
                // Add overlap
                var words = overlapText.Split(' ');
                for (int i = Math.Max(0, words.Length - overlap / 5); i < words.Length; i++)
                {
                    currentChunk.Append(words[i]);
                    currentChunk.Append(' ');
                }
            }
            currentChunk.Append(sentence);
            currentChunk.Append(". ");
        }

        if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString().Trim());
        }

        return chunks;
    }

    private string ExtractText(byte[] fileBytes, string mimeType)
    {
        try
        {
            return mimeType.ToLower() switch
            {
                "text/plain" or "text/markdown" => Encoding.UTF8.GetString(fileBytes),
                _ => Encoding.UTF8.GetString(fileBytes) // Simplified - in production use proper parsers
            };
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ComputeSHA256(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }

    private static string GetMimeType(string extension)
    {
        return extension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".doc" => "application/msword",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".txt" => "text/plain",
            ".md" => "text/markdown",
            _ => "application/octet-stream"
        };
    }
}
