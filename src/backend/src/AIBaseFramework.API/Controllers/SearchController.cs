// Search Controller - Semantic Search Endpoints
using AIBaseFramework.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIBaseFramework.API.Controllers;

[ApiController]
[Route("api/v1/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int topK = 10,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Query không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var results = await _searchService.SearchAsync(q, userId, topK, categoryId, fromDate, toDate);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed for query: {Query}", q);
            return StatusCode(500, new { message = "Tìm kiếm thất bại" });
        }
    }

    [HttpPost("hybrid")]
    public async Task<IActionResult> HybridSearch([FromBody] HybridSearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { message = "Query không được để trống" });

        try
        {
            var userId = GetCurrentUserId();
            var results = await _searchService.HybridSearchAsync(
                request.Query,
                userId,
                request.TopK,
                request.VectorWeight,
                request.KeywordWeight);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hybrid search failed for query: {Query}", request.Query);
            return StatusCode(500, new { message = "Tìm kiếm thất bại" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

public record HybridSearchRequest(
    string Query,
    int TopK = 10,
    float VectorWeight = 0.7f,
    float KeywordWeight = 0.3f);
