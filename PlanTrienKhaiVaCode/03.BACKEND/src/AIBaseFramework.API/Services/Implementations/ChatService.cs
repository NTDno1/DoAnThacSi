// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: Chat Service Implementation
// ============================================================

using AIBaseFramework.API.Domain.Entities;
using AIBaseFramework.API.Infrastructure.AI;
using AIBaseFramework.API.Infrastructure.Data;
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIBaseFramework.API.Services.Implementations;

public class ChatService : IChatService
{
    private readonly AppDbContext _dbContext;
    private readonly IRAGPipeline _ragPipeline;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        AppDbContext dbContext,
        IRAGPipeline ragPipeline,
        ILogger<ChatService> logger)
    {
        _dbContext = dbContext;
        _ragPipeline = ragPipeline;
        _logger = logger;
    }

    public async Task<List<ChatSessionDto>> GetSessionsAsync(Guid userId)
    {
        var sessions = await _dbContext.ChatSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.UpdatedAt)
            .Take(50)
            .Select(s => new ChatSessionDto(
                s.Id,
                s.Title,
                s.MessageCount,
                s.LastMessagePreview,
                s.CreatedAt))
            .ToListAsync();

        return sessions;
    }

    public async Task<ChatSessionDto> CreateSessionAsync(Guid userId, string? title = null)
    {
        var session = new ChatSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title ?? $"Cuộc trò chuyện mới - {DateTime.Now:yyyy-MM-dd HH:mm}",
            MessageCount = 0,
            IsArchived = false,
            IsPinned = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.ChatSessions.Add(session);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Chat session created: {Id}", session.Id);

        return new ChatSessionDto(
            session.Id,
            session.Title,
            session.MessageCount,
            session.LastMessagePreview,
            session.CreatedAt);
    }

    public async Task<ChatSessionDto?> GetSessionAsync(Guid id, Guid userId)
    {
        var session = await _dbContext.ChatSessions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (session == null) return null;

        return new ChatSessionDto(
            session.Id,
            session.Title,
            session.MessageCount,
            session.LastMessagePreview,
            session.CreatedAt);
    }

    public async Task DeleteSessionAsync(Guid id, Guid userId)
    {
        var session = await _dbContext.ChatSessions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (session == null)
        {
            throw new UnauthorizedAccessException("Không có quyền xóa phiên chat này");
        }

        // Delete messages
        var messages = await _dbContext.ChatMessages
            .Where(m => m.SessionId == id)
            .ToListAsync();
        _dbContext.ChatMessages.RemoveRange(messages);

        // Delete session
        _dbContext.ChatSessions.Remove(session);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Chat session deleted: {Id}", id);
    }

    public async Task<ChatMessageResponse> SendMessageAsync(Guid sessionId, string content, Guid userId)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var session = await _dbContext.ChatSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

        if (session == null)
        {
            throw new UnauthorizedAccessException("Không có quyền truy cập phiên chat này");
        }

        // Save user message
        var userMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            Role = MessageRole.User,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.ChatMessages.Add(userMessage);

        // Get conversation history
        var history = await _dbContext.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(10)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        // Build context for RAG
        var ragResponse = await _ragPipeline.GenerateResponseAsync(
            content, 
            sessionId, 
            System.Text.Json.JsonSerializer.Serialize(history.Select(m => new { m.Role, m.Content })));

        // Save assistant message
        var assistantMessage = new ChatMessage
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            Role = MessageRole.Assistant,
            Content = ragResponse.Answer,
            Citations = System.Text.Json.JsonSerializer.Serialize(ragResponse.Sources),
            ModelUsed = "llama3.2:3b",
            TokenCount = ragResponse.TokensUsed,
            LatencyMs = ragResponse.LatencyMs,
            SourcesUsed = "[]",
            TokenUsage = "{}",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.ChatMessages.Add(assistantMessage);

        // Update session
        session.MessageCount += 2;
        session.LastMessagePreview = content.Length > 100 ? content.Substring(0, 100) + "..." : content;
        session.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        stopwatch.Stop();

        // Build sources
        var sources = ragResponse.Sources.Select(s => new RAGSourceDto(
            s.DocumentId,
            s.DocumentTitle,
            s.PageNumber,
            s.Excerpt.Length > 200 ? s.Excerpt.Substring(0, 200) + "..." : s.Excerpt,
            s.RelevanceScore
        )).ToList();

        return new ChatMessageResponse(
            ragResponse.Answer,
            sources,
            ragResponse.Confidence,
            ragResponse.LatencyMs);
    }

    public async Task<List<ChatMessageDto>> GetMessagesAsync(Guid sessionId, Guid userId, int limit = 50)
    {
        // Verify access
        var session = await _dbContext.ChatSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

        if (session == null)
        {
            throw new UnauthorizedAccessException("Không có quyền truy cập phiên chat này");
        }

        var messages = await _dbContext.ChatMessages
            .Where(m => m.SessionId == sessionId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(limit)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageDto(
                m.Id,
                m.Role.ToString().ToLower(),
                m.Content,
                m.Citations,
                m.CreatedAt))
            .ToListAsync();

        return messages;
    }
}
