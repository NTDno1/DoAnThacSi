// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Admin Controller - Dashboard & System Management
// ============================================================

using AIBaseFramework.API.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIBaseFramework.API.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    // ============================================
    // DASHBOARD
    // ============================================

    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get dashboard stats");
            return StatusCode(500, new { message = "Không thể lấy thống kê dashboard" });
        }
    }

    [HttpGet("dashboard/activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 20)
    {
        try
        {
            var activities = await _adminService.GetRecentActivitiesAsync(limit);
            return Ok(activities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent activities");
            return StatusCode(500, new { message = "Không thể lấy hoạt động gần đây" });
        }
    }

    [HttpGet("dashboard/search-trends")]
    public async Task<IActionResult> GetSearchTrends([FromQuery] int days = 7)
    {
        try
        {
            var trends = await _adminService.GetSearchTrendsAsync(days);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get search trends");
            return StatusCode(500, new { message = "Không thể lấy xu hướng tìm kiếm" });
        }
    }

    // ============================================
    // USER MANAGEMENT
    // ============================================

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? role = null,
        [FromQuery] string? search = null)
    {
        try
        {
            var users = await _adminService.GetAllUsersAsync(page, pageSize, role, search);
            return Ok(new { items = users, total = await _adminService.GetTotalUsersCountAsync() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get users");
            return StatusCode(500, new { message = "Không thể lấy danh sách người dùng" });
        }
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user {Id}", id);
            return StatusCode(500, new { message = "Không thể lấy thông tin người dùng" });
        }
    }

    [HttpPut("users/{id:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var user = await _adminService.UpdateUserRoleAsync(id, request.Role);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user role {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("users/{id:guid}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(Guid id)
    {
        try
        {
            var isActive = await _adminService.ToggleUserStatusAsync(id);
            return Ok(new { userId = id, isActive });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle user status {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    // ============================================
    // DOCUMENT STATISTICS
    // ============================================

    [HttpGet("documents/by-category")]
    public async Task<IActionResult> GetDocumentsByCategory()
    {
        try
        {
            var stats = await _adminService.GetDocumentsByCategoryAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get documents by category");
            return StatusCode(500, new { message = "Không thể lấy thống kê tài liệu" });
        }
    }

    [HttpGet("documents/by-status")]
    public async Task<IActionResult> GetDocumentsByStatus()
    {
        try
        {
            var stats = await _adminService.GetDocumentsByStatusAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get documents by status");
            return StatusCode(500, new { message = "Không thể lấy thống kê tài liệu" });
        }
    }

    [HttpGet("documents/recent")]
    public async Task<IActionResult> GetRecentDocuments([FromQuery] int limit = 10)
    {
        try
        {
            var documents = await _adminService.GetRecentDocumentsAsync(limit);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent documents");
            return StatusCode(500, new { message = "Không thể lấy tài liệu gần đây" });
        }
    }

    // ============================================
    // SYSTEM HEALTH
    // ============================================

    [HttpGet("health")]
    public async Task<IActionResult> GetSystemHealth()
    {
        try
        {
            var health = await _adminService.GetSystemHealthAsync();
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get system health");
            return StatusCode(500, new { message = "Không thể kiểm tra trạng thái hệ thống" });
        }
    }

    [HttpGet("health/providers")]
    public async Task<IActionResult> GetAIProvidersHealth()
    {
        try
        {
            var providers = await _adminService.GetAIProvidersHealthAsync();
            return Ok(providers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get AI providers health");
            return StatusCode(500, new { message = "Không thể kiểm tra trạng thái AI providers" });
        }
    }

    // ============================================
    // AUDIT LOGS
    // ============================================

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? action = null,
        [FromQuery] Guid? userId = null)
    {
        try
        {
            var logs = await _adminService.GetAuditLogsAsync(page, pageSize, action, userId);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get audit logs");
            return StatusCode(500, new { message = "Không thể lấy audit logs" });
        }
    }
}

// Request DTOs
public record UpdateRoleRequest(string Role);
