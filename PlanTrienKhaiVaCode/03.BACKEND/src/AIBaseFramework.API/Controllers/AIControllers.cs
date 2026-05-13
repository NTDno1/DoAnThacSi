// ============================================================
// AI BASE FRAMEWORK - ENTERPRISE AI PLATFORM
// Module: AI Controllers - API Endpoints
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIBaseFramework.AI.Agent;
using AIBaseFramework.AI.RAG;
using AIBaseFramework.AI.Search;
using AIBaseFramework.AI.Voice;
using AIBaseFramework.AI.SQL;
using AIBaseFramework.AI.Gateway;

namespace AIBaseFramework.API.Controllers;

// ============================================================
// AI CONTROLLER
// ============================================================

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAI Gateway _aiGateway;
    private readonly ILogger<AIController> _logger;
    
    public AIController(IAI Gateway aiGateway, ILogger<AIController> logger)
    {
        _aiGateway = aiGateway;
        _logger = logger;
    }
    
    /// <summary>
    /// Chat with AI (direct)
    /// </summary>
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _aiGateway.ChatAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Stream chat response
    /// </summary>
    [HttpPost("chat/stream")]
    public async Task StreamChat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        
        try
        {
            await foreach (var delta in _aiGateway.StreamChatAsync(request, cancellationToken))
            {
                if (delta.Content != null)
                {
                    await Response.WriteAsync($"data: {delta.Content}\n\n", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stream chat failed");
            await Response.WriteAsync($"data: ERROR: {ex.Message}\n\n", cancellationToken);
        }
    }
    
    /// <summary>
    /// Generate embeddings
    /// </summary>
    [HttpPost("embed")]
    public async Task<IActionResult> Embed([FromBody] EmbedRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _aiGateway.EmbedAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Embedding failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get all available models
    /// </summary>
    [HttpGet("models")]
    public async Task<IActionResult> GetModels(CancellationToken cancellationToken)
    {
        try
        {
            var models = await _aiGateway.GetAllAvailableModelsAsync(cancellationToken);
            return Ok(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get models");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get provider health status
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        try
        {
            var health = await _aiGateway.GetAllProviderHealthAsync(cancellationToken);
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

// ============================================================
// RAG CONTROLLER
// ============================================================

[ApiController]
[Route("api/v1/rag")]
[Authorize]
public class RAGController : ControllerBase
{
    private readonly IRAGPipeline _ragPipeline;
    private readonly ILogger<RAGController> _logger;
    
    public RAGController(IRAGPipeline ragPipeline, ILogger<RAGController> logger)
    {
        _ragPipeline = ragPipeline;
        _logger = logger;
    }
    
    /// <summary>
    /// Query with RAG
    /// </summary>
    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] RAGRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            request.UserId = userId;
            request.TenantId = tenantId;
            
            var response = await _ragPipeline.QueryAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RAG query failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Stream RAG query
    /// </summary>
    [HttpPost("query/stream")]
    public async Task StreamQuery([FromBody] RAGRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        
        try
        {
            await foreach (var chunk in _ragPipeline.QueryStreamAsync(request, cancellationToken))
            {
                await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RAG stream failed");
            await Response.WriteAsync($"data: ERROR: {ex.Message}\n\n", cancellationToken);
        }
    }
}

// ============================================================
// SEARCH CONTROLLER
// ============================================================

[ApiController]
[Route("api/v1/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchEngine _searchEngine;
    private readonly ILogger<SearchController> _logger;
    
    public SearchController(ISearchEngine searchEngine, ILogger<SearchController> logger)
    {
        _searchEngine = searchEngine;
        _logger = logger;
    }
    
    /// <summary>
    /// Search documents
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] SearchType searchType = SearchType.Hybrid,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            var request = new SearchRequest
            {
                Query = q,
                SearchType = searchType,
                Limit = limit,
                UserId = userId,
                TenantId = tenantId
            };
            
            var response = await _searchEngine.SearchAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Search with filters
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SearchAdvanced([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            request.UserId = userId;
            request.TenantId = tenantId;
            
            var response = await _searchEngine.SearchAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Advanced search failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Autocomplete
    /// </summary>
    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete(
        [FromQuery] string prefix,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var suggestions = await _searchEngine.AutocompleteAsync(prefix, limit, cancellationToken);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Autocomplete failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

// ============================================================
// AGENT CONTROLLER
// ============================================================

[ApiController]
[Route("api/v1/agent")]
[Authorize]
public class AgentController : ControllerBase
{
    private readonly IAgentOrchestrator _agentOrchestrator;
    private readonly ILogger<AgentController> _logger;
    
    public AgentController(IAgentOrchestrator agentOrchestrator, ILogger<AgentController> logger)
    {
        _agentOrchestrator = agentOrchestrator;
        _logger = logger;
    }
    
    /// <summary>
    /// Process agent request
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] AgentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            request.UserId = userId;
            request.TenantId = tenantId;
            
            var response = await _agentOrchestrator.ProcessAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent processing failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Stream agent response
    /// </summary>
    [HttpPost("process/stream")]
    public async Task StreamProcess([FromBody] AgentRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            request.UserId = userId;
            request.TenantId = tenantId;
            request.StreamResponse = true;
            
            await foreach (var chunk in _agentOrchestrator.StreamProcessAsync(request, cancellationToken))
            {
                await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent stream failed");
            await Response.WriteAsync($"data: ERROR: {ex.Message}\n\n", cancellationToken);
        }
    }
    
    /// <summary>
    /// Create new session
    /// </summary>
    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            var session = await _agentOrchestrator.CreateSessionAsync(userId, tenantId);
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create session");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get session
    /// </summary>
    [HttpGet("sessions/{sessionId}")]
    public async Task<IActionResult> GetSession(string sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var session = await _agentOrchestrator.GetSessionAsync(sessionId);
            
            if (session == null)
            {
                return NotFound();
            }
            
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get session");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Approve action
    /// </summary>
    [HttpPost("sessions/{sessionId}/actions/{actionId}/approve")]
    public async Task<IActionResult> ApproveAction(
        string sessionId,
        string actionId,
        [FromBody] ApprovalRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var action = await _agentOrchestrator.ApproveActionAsync(
                sessionId, 
                actionId, 
                request.Approved, 
                request.ModifiedParameters);
            
            return Ok(action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to approve action");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

/// <summary>
/// Approval request
/// </summary>
public class ApprovalRequest
{
    public bool Approved { get; set; }
    public Dictionary<string, object>? ModifiedParameters { get; set; }
}

// ============================================================
// SQL CONTROLLER
// ============================================================

[ApiController]
[Route("api/v1/sql")]
[Authorize]
public class SQLController : ControllerBase
{
    private readonly ISQLEngine _sqlEngine;
    private readonly ILogger<SQLController> _logger;
    
    public SQLController(ISQLEngine sqlEngine, ILogger<SQLController> logger)
    {
        _sqlEngine = sqlEngine;
        _logger = logger;
    }
    
    /// <summary>
    /// Execute natural language SQL query
    /// </summary>
    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] SQLQueryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;
            
            request.UserId = userId;
            request.TenantId = tenantId;
            
            var response = await _sqlEngine.QueryAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQL query failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Generate SQL from natural language (without execution)
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateSQL([FromBody] GenerateSQLRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var sql = await _sqlEngine.GenerateSQLAsync(request.Query, userId, cancellationToken);
            
            return Ok(new { sql });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQL generation failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get available tables and schema
    /// </summary>
    [HttpGet("schema")]
    public async Task<IActionResult> GetSchema(CancellationToken cancellationToken)
    {
        try
        {
            var tables = _sqlEngine.GetAvailableTables();
            var description = _sqlEngine.GetSchemaDescription();
            
            return Ok(new { tables, schema = description });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get schema");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

/// <summary>
/// Generate SQL request
/// </summary>
public class GenerateSQLRequest
{
    public string Query { get; set; } = string.Empty;
}

// ============================================================
// VOICE CONTROLLER
// ============================================================

[ApiController]
[Route("api/v1/voice")]
[Authorize]
public class VoiceController : ControllerBase
{
    private readonly IVoiceService _voiceService;
    private readonly ILogger<VoiceController> _logger;
    
    public VoiceController(IVoiceService voiceService, ILogger<VoiceController> logger)
    {
        _voiceService = voiceService;
        _logger = logger;
    }
    
    /// <summary>
    /// Transcribe audio to text
    /// </summary>
    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe(IFormFile audio, CancellationToken cancellationToken)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await audio.CopyToAsync(memoryStream, cancellationToken);
            
            var request = new STTRequest
            {
                AudioData = memoryStream.ToArray(),
                Format = Path.GetExtension(audio.FileName).TrimStart('.'),
                Language = Request.Form["language"].FirstOrDefault() ?? "vi"
            };
            
            var response = await _voiceService.TranscribeAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transcription failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Synthesize text to speech
    /// </summary>
    [HttpPost("synthesize")]
    public async Task<IActionResult> Synthesize([FromBody] TTSRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _voiceService.SynthesizeAsync(request, cancellationToken);
            
            return File(response.AudioData, GetMimeType(response.Format), "audio." + response.Format);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synthesis failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Voice to action (voice command)
    /// </summary>
    [HttpPost("command")]
    public async Task<IActionResult> VoiceCommand(IFormFile audio, CancellationToken cancellationToken)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await audio.CopyToAsync(memoryStream, cancellationToken);
            
            var intent = await _voiceService.RecognizeIntentAsync(memoryStream.ToArray(), cancellationToken);
            return Ok(intent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Voice command failed");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    private static string GetMimeType(string format)
    {
        return format.ToLower() switch
        {
            "mp3" => "audio/mpeg",
            "wav" => "audio/wav",
            "ogg" => "audio/ogg",
            "opus" => "audio/opus",
            _ => "application/octet-stream"
        };
    }
}
