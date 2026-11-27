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

    public SectionController (SectionService sectionService)
    {
        _sectionService = sectionService;
    }

    // GET api/section/venue/{venueId} -> o mekânın tüm bölümleri
    [HttpGet("venue/{venueId}")]
    public async Task<IActionResult> GetSectionsByVenue (string venueId)
        => Ok(await _sectionService.GetSectionsByVenueAsync(venueId));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNewSection ([FromBody] CreateSectionDto dto)
    {
        var created = await _sectionService.CreateVenueSectionAsync(dto);
        // HashedId: created.Id gerçek int id, Location header'ında dışarı sızmamalı
        return Created($"/api/section/{created.HashedId}", created);
    }
}
