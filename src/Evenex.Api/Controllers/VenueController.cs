using Evenex.Application.Abstractions;
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
    private readonly ICurrentUserService _currentUser;

    public VenueController (VenueService venueService, ICurrentUserService currentUser)
    {
        _venueService = venueService;
        _currentUser = currentUser;
    }

    // İsteği yapan istemcinin IP adresi (audit alanları için: CreatedIP / ModifiedIP)
    private string ClientIp => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll () => Ok(await _venueService.GetAllVenuesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVenueById (string id) => Ok(await _venueService.GetByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNewVenue ([FromBody] CreateVenueDto dto)
    {
        var created = await _venueService.CreateVenueAsync(dto, _currentUser.Email, ClientIp);

        // 201 Created + "Location: /api/venue/{id}" header'ı + oluşan kaydın kendisi
        return CreatedAtAction(nameof(GetVenueById), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVenue (string id, [FromBody] UpdateVenueDto dto)
        => Ok(await _venueService.UpdateAsync(id, dto, _currentUser.Email, ClientIp));

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVenue (string id)
    {
        await _venueService.DeleteAsync(id);
        return NoContent(); // 204: başarılı, geri dönecek içerik yok
    }
}
