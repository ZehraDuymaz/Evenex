using Evenex.Common.Hashing;
using Evenex.Application.Venues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

/// <summary>
/// Mekân endpoint'leri: localhost:5177/api/venue
/// Okuma herkese açık; oluşturma/güncelleme/silme için geçerli bir JWT gerekir ([Authorize]).
/// Hatalar (bulunamadı, geçersiz id vs.) GlobalExceptionHandler tarafından HTTP koduna çevrilir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly VenueService _venueService;

    public VenueController (VenueService venueService)
    {
        _venueService = venueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll () => Ok(await _venueService.GetAllVenuesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVenueById ([HashId] int id) => Ok(await _venueService.GetByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNewVenue ([FromBody] CreateVenueDto dto)
    {
        var created = await _venueService.CreateVenueAsync(dto);

        // 201 Created + "Location: /api/venue/{id}" header'ı + oluşan kaydın kendisi
        // Location header'ına hash'li id: created.Id gerçek int id'dir, dışarı sızmamalı
        return CreatedAtAction(nameof(GetVenueById), new { id = created.HashedId }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVenue ([HashId] int id, [FromBody] UpdateVenueDto dto)
        => Ok(await _venueService.UpdateAsync(id, dto));

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVenue ([HashId] int id)
    {
        await _venueService.DeleteAsync(id);
        return NoContent(); // 204: başarılı, geri dönecek içerik yok
    }
}
