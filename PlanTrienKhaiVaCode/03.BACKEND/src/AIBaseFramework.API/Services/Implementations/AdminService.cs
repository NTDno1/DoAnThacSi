// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Admin Service - Dashboard & System Management
// ============================================================

using AIBaseFramework.API.Domain.Entities;
using AIBaseFramework.API.Infrastructure.Data;
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AIBaseFramework.API.Services.Implementations;

public interface IAdminService
{
    // Dashboard Statistics
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<List<ActivityLogDto>> GetRecentActivitiesAsync(int limit = 20);
    Task<List<SearchTrendDto>> GetSearchTrendsAsync(int days = 7);
    
    // User Management
    Task<List<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? role = null, string? search = null);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserDto> UpdateUserRoleAsync(Guid userId, string newRole);
    Task<bool> ToggleUserStatusAsync(Guid userId);
    Task<int> GetTotalUsersCountAsync();
    
    // Document Management
    Task<List<DocumentStatsDto>> GetDocumentsByCategoryAsync();
    Task<List<DocumentStatsDto>> GetDocumentsByStatusAsync();
    Task<List<DocumentDto>> GetRecentDocumentsAsync(int limit = 10);
    
    // System Health
    Task<SystemHealthDto> GetSystemHealthAsync();
    Task<List<AIProviderHealthDto>> GetAIProvidersHealthAsync();
    
    // Audit
    Task<List<AuditLogDto>> GetAuditLogsAsync(int page = 1, int pageSize = 50, string? action = null, Guid? userId = null);
}

// Dashboard Statistics
public class DashboardStats
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalDocuments { get; set; }
    public int IndexedDocuments { get; set; }
    public int TotalChunks { get; set; }
    public int TotalSearches { get; set; }
    public int SearchesToday { get; set; }
    public int TotalChatSessions { get; set; }
    public int TotalMessages { get; set; }
    public double AvgSearchTimeMs { get; set; }
    public double StorageUsedGB { get; set; }
    public List<DailyStatsDto> DailyStats { get; set; } = new();
}

public class DailyStatsDto
{
    public DateTime Date { get; set; }
    public int Searches { get; set; }
    public int Documents { get; set; }
    public int Users { get; set; }
}

public class ActivityLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SearchTrendDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public double AvgTimeMs { get; set; }
    public int ResultsFound { get; set; }
}

public class DocumentStatsDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public long TotalSize { get; set; }
}

public class SystemHealthDto
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; } = "Unknown";
    public DateTime CheckedAt { get; set; }
    public Dictionary<string, ComponentHealthDto> Components { get; set; } = new();
}

public class ComponentHealthDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string? Message { get; set; }
    public double? LatencyMs { get; set; }
}

public class AIProviderHealthDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string? Model { get; set; }
    public int? Load { get; set; }
    public string? Message { get; set; }
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Details { get; set; }
    public string? UserEmail { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminService : IAdminService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AdminService> _logger;

    public AdminService(AppDbContext dbContext, ILogger<AdminService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var stats = new DashboardStats();
        
        // User stats
        stats.TotalUsers = await _dbContext.Users.CountAsync();
        stats.ActiveUsers = await _dbContext.Users.CountAsync(u => u.IsActive);
        
        // Document stats
        stats.TotalDocuments = await _dbContext.Documents.CountAsync();
        stats.IndexedDocuments = await _dbContext.Documents.CountAsync(d => d.Status == DocumentStatus.Indexed);
        
        // Chunk stats
        stats.TotalChunks = await _dbContext.DocumentChunks.CountAsync();
        
        // Search stats
        stats.TotalSearches = await _dbContext.SearchLogs.CountAsync();
        stats.SearchesToday = await _dbContext.SearchLogs
            .CountAsync(s => s.CreatedAt >= DateTime.UtcNow.Date);
        stats.AvgSearchTimeMs = await _dbContext.SearchLogs.AnyAsync()
            ? await _dbContext.SearchLogs.AverageAsync(s => s.LatencyMs ?? 0)
            : 0;
        
        // Chat stats
        stats.TotalChatSessions = await _dbContext.ChatSessions.CountAsync();
        stats.TotalMessages = await _dbContext.ChatMessages.CountAsync();
        
        // Storage estimate
        var totalSize = await _dbContext.Documents.SumAsync(d => d.FileSize);
        stats.StorageUsedGB = totalSize / (1024.0 * 1024 * 1024);
        
        // Daily stats for last 7 days
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.Date.AddDays(-i))
            .ToList();

        foreach (var date in last7Days)
        {
            var nextDate = date.AddDays(1);
            stats.DailyStats.Add(new DailyStatsDto
            {
                Date = date,
                Searches = await _dbContext.SearchLogs.CountAsync(s => s.CreatedAt >= date && s.CreatedAt < nextDate),
                Documents = await _dbContext.Documents.CountAsync(d => d.CreatedAt >= date && d.CreatedAt < nextDate),
                Users = await _dbContext.Users.CountAsync(u => u.CreatedAt >= date && u.CreatedAt < nextDate)
            });
        }
        
        stats.DailyStats = stats.DailyStats.OrderByDescending(s => s.Date).ToList();

        return stats;
    }

    public async Task<List<ActivityLogDto>> GetRecentActivitiesAsync(int limit = 20)
    {
        return await _dbContext.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(a => new ActivityLogDto
            {
                Id = a.Id,
                Action = a.Action,
                UserEmail = a.User != null ? a.User.Email : "System",
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<SearchTrendDto>> GetSearchTrendsAsync(int days = 7)
    {
        var result = new List<SearchTrendDto>();
        
        for (int i = days - 1; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var nextDate = date.AddDays(1);
            
            var dayLogs = await _dbContext.SearchLogs
                .Where(s => s.CreatedAt >= date && s.CreatedAt < nextDate)
                .ToListAsync();
            
            result.Add(new SearchTrendDto
            {
                Date = date,
                Count = dayLogs.Count,
                AvgTimeMs = dayLogs.Any() ? dayLogs.Average(s => s.LatencyMs ?? 0) : 0,
                ResultsFound = dayLogs.Sum(s => s.ResultsCount)
            });
        }
        
        return result;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20, string? role = null, string? search = null)
    {
        var query = _dbContext.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            query = query.Where(u => u.Role == userRole);
        }
        
        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(u => 
                u.Email.ToLower().Contains(search) || 
                u.FullName.ToLower().Contains(search));
        }
        
        return await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto(u.Id, u.Email, u.FullName, u.Department, u.Role.ToString()))
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;
        
        return new UserDto(user.Id, user.Email, user.FullName, user.Department, user.Role.ToString());
    }

    public async Task<UserDto> UpdateUserRoleAsync(Guid userId, string newRole)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("Không tìm thấy người dùng");
        
        if (!Enum.TryParse<UserRole>(newRole, true, out var role))
            throw new Exception("Vai trò không hợp lệ");
        
        user.Role = role;
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} role updated to {Role}", userId, newRole);
        
        return new UserDto(user.Id, user.Email, user.FullName, user.Department, user.Role.ToString());
    }

    public async Task<bool> ToggleUserStatusAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("Không tìm thấy người dùng");
        
        user.IsActive = !user.IsActive;
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("User {UserId} status toggled to {Status}", userId, user.IsActive);
        
        return user.IsActive;
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        return await _dbContext.Users.CountAsync();
    }

    public async Task<List<DocumentStatsDto>> GetDocumentsByCategoryAsync()
    {
        return await _dbContext.DocumentCategories
            .Select(c => new DocumentStatsDto
            {
                Name = c.Name,
                Count = c.Documents.Count,
                TotalSize = c.Documents.Sum(d => d.FileSize)
            })
            .ToListAsync();
    }

    public async Task<List<DocumentStatsDto>> GetDocumentsByStatusAsync()
    {
        return await _dbContext.Documents
            .GroupBy(d => d.Status.ToString())
            .Select(g => new DocumentStatsDto
            {
                Name = g.Key,
                Count = g.Count(),
                TotalSize = g.Sum(d => d.FileSize)
            })
            .ToListAsync();
    }

    public async Task<List<DocumentDto>> GetRecentDocumentsAsync(int limit = 10)
    {
        return await _dbContext.Documents
            .Include(d => d.Category)
            .OrderByDescending(d => d.CreatedAt)
            .Take(limit)
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
    }

    public async Task<SystemHealthDto> GetSystemHealthAsync()
    {
        var health = new SystemHealthDto
        {
            CheckedAt = DateTime.UtcNow,
            Components = new Dictionary<string, ComponentHealthDto>()
        };

        try
        {
            // Check database
            var canConnect = await _dbContext.Database.CanConnectAsync();
            health.Components["Database"] = new ComponentHealthDto
            {
                Name = "PostgreSQL",
                IsHealthy = canConnect,
                Message = canConnect ? "Connected" : "Connection failed"
            };

            // Check documents table
            var docCount = await _dbContext.Documents.CountAsync();
            health.Components["Documents"] = new ComponentHealthDto
            {
                Name = "Documents",
                IsHealthy = true,
                Message = $"{docCount} documents"
            };

            health.IsHealthy = canConnect;
            health.Status = canConnect ? "Healthy" : "Degraded";
        }
        catch (Exception ex)
        {
            health.IsHealthy = false;
            health.Status = "Error";
            health.Components["Error"] = new ComponentHealthDto
            {
                Name = "System",
                IsHealthy = false,
                Message = ex.Message
            };
        }

        return health;
    }

    public async Task<List<AIProviderHealthDto>> GetAIProvidersHealthAsync()
    {
        var providers = new List<AIProviderHealthDto>();

        // Ollama status (simplified - in production, call actual health check)
        providers.Add(new AIProviderHealthDto
        {
            Name = "Ollama",
            IsHealthy = true,
            Model = "llama3.2:3b"
        });

        // Add other providers as needed
        providers.Add(new AIProviderHealthDto
        {
            Name = "Embedding",
            IsHealthy = true,
            Model = "nomic-embed-text"
        });

        return providers;
    }

    public async Task<List<AuditLogDto>> GetAuditLogsAsync(int page = 1, int pageSize = 50, string? action = null, Guid? userId = null)
    {
        var query = _dbContext.AuditLogs.AsQueryable();
        
        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(a => a.Action.Contains(action));
        }
        
        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }
        
        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Details = a.Details,
                UserEmail = a.User != null ? a.User.Email : null,
                IpAddress = a.IpAddress,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }
}
