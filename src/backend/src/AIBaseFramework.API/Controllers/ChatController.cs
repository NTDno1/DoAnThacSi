// Chat Controller - RAG Chatbot Endpoints
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIBaseFramework.API.Controllers;

[ApiController]
[Route("api/v1/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        var userId = GetCurrentUserId();
        var sessions = await _chatService.GetSessionsAsync(userId);
        return Ok(sessions);
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest? request = null)
    {
        var userId = GetCurrentUserId();
        var session = await _chatService.CreateSessionAsync(userId, request?.Title);
        return Created($"/api/v1/chat/sessions/{session.Id}", session);
    }

    [HttpGet("sessions/{id:guid}")]
    public async Task<IActionResult> GetSession(Guid id)
    {
        var userId = GetCurrentUserId();
        var session = await _chatService.GetSessionAsync(id, userId);
        
        if (session == null)
            return NotFound(new { message = "Không tìm thấy phiên chat" });

        return Ok(session);
    }

    [HttpDelete("sessions/{id:guid}")]
    public async Task<IActionResult> DeleteSession(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _chatService.DeleteSessionAsync(id, userId);
            return Ok(new { message = "Xóa phiên chat thành công" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Session deletion failed: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("sessions/{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Tin nhắn không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var response = await _chatService.SendMessageAsync(id, request.Content, userId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send message failed for session: {Id}", id);
            return StatusCode(500, new { message = "Gửi tin nhắn thất bại" });
        }
    }

    [HttpGet("sessions/{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, [FromQuery] int limit = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            var messages = await _chatService.GetMessagesAsync(id, userId, limit);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

public record CreateSessionRequest(string? Title);
public record SendMessageRequest(string Content);
