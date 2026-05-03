// Documents Controller - Document CRUD Endpoints
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIBaseFramework.API.Controllers;

[ApiController]
[Route("api/v1/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(52428800)] // 50MB
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] Guid? categoryId = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _documentService.UploadDocumentAsync(file, userId, categoryId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document upload failed");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? status = null)
    {
        var userId = GetCurrentUserId();
        var result = await _documentService.GetDocumentsAsync(userId, page, pageSize, categoryId, status);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var userId = GetCurrentUserId();
        var document = await _documentService.GetDocumentByIdAsync(id, userId);
        
        if (document == null)
            return NotFound(new { message = "Không tìm thấy tài liệu" });

        return Ok(document);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _documentService.DeleteDocumentAsync(id, userId);
            return Ok(new { message = "Xóa tài liệu thành công" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document deletion failed: {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetDocumentStatus(Guid id)
    {
        var status = await _documentService.GetDocumentStatusAsync(id);
        return Ok(new { id, status });
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var (stream, fileName, contentType) = await _documentService.DownloadDocumentAsync(id, userId);
            return File(stream, contentType, fileName);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document download failed: {Id}", id);
            return NotFound(new { message = "Không tìm thấy tài liệu" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
