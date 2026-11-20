using Evenex.Application.Abstractions;
using Evenex.Application.VenueSections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

/// <summary>
/// Mekân bölümü endpoint'leri: localhost:5177/api/section
/// Bölümler her zaman bir mekâna bağlı olduğu için listeleme mekân id'si ile yapılır.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SectionController : ControllerBase
{
    private readonly SectionService _sectionService;
    private readonly ICurrentUserService _currentUser;

    public SectionController (SectionService sectionService, ICurrentUserService currentUser)
    {
        _sectionService = sectionService;
        _currentUser = currentUser;
    }

    private string ClientIp => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    // GET api/section/venue/{venueId} -> o mekânın tüm bölümleri
    [HttpGet("venue/{venueId}")]
    public async Task<IActionResult> GetSectionsByVenue (string venueId)
        => Ok(await _sectionService.GetSectionsByVenueAsync(venueId));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNewSection ([FromBody] CreateSectionDto dto)
    {
        var created = await _sectionService.CreateVenueSectionAsync(dto, _currentUser.Email, ClientIp);
        return Created($"/api/section/{created.Id}", created);
    }
}
