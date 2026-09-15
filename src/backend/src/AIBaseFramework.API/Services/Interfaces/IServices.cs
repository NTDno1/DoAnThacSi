// Service Interfaces
namespace AIBaseFramework.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
}

public interface IDocumentService
{
    Task<DocumentUploadResult> UploadDocumentAsync(IFormFile file, Guid userId, Guid? categoryId = null);
    Task<PagedResult<DocumentDto>> GetDocumentsAsync(Guid userId, int page, int pageSize, Guid? categoryId = null, string? status = null);
    Task<DocumentDto?> GetDocumentByIdAsync(Guid id, Guid userId);
    Task DeleteDocumentAsync(Guid id, Guid userId);
    Task<string> GetDocumentStatusAsync(Guid id);
    Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(Guid id, Guid userId);
}

public interface ISearchService
{
    Task<List<SearchResult>> SearchAsync(string query, Guid userId, int topK = 10, Guid? categoryId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<SearchResult>> HybridSearchAsync(string query, Guid userId, int topK = 10, float vectorWeight = 0.7f, float keywordWeight = 0.3f);
}

public interface IChatService
{
    Task<List<ChatSessionDto>> GetSessionsAsync(Guid userId);
    Task<ChatSessionDto> CreateSessionAsync(Guid userId, string? title = null);
    Task<ChatSessionDto?> GetSessionAsync(Guid id, Guid userId);
    Task DeleteSessionAsync(Guid id, Guid userId);
    Task<ChatMessageResponse> SendMessageAsync(Guid sessionId, string content, Guid userId);
    Task<List<ChatMessageDto>> GetMessagesAsync(Guid sessionId, Guid userId, int limit = 50);
}

// DTOs
public record RegisterRequest(string Email, string Password, string FullName, string? Department = null);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, string RefreshToken, UserDto User);
public record UserDto(Guid Id, string Email, string FullName, string? Department, string Role);

public record DocumentUploadResult(Guid Id, string Title, string Status, DateTime CreatedAt);
public record DocumentDto(Guid Id, string Title, string OriginalFilename, long FileSize, string MimeType, string Status, DateTime CreatedAt, Guid? CategoryId, string? CategoryName);
public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize, int TotalPages);

public record SearchResult(Guid DocumentId, string Title, string Excerpt, float Score, int? PageNumber, string? Source);
public record ChatSessionDto(Guid Id, string Title, int MessageCount, string? LastMessagePreview, DateTime CreatedAt);
public record ChatMessageDto(Guid Id, string Role, string Content, string? Citations, DateTime CreatedAt);
public record ChatMessageResponse(string Content, List<RAGSourceDto> Sources, float Confidence, int LatencyMs);

public record RAGSourceDto(Guid DocumentId, string Title, int PageNumber, string Excerpt, float Score);
