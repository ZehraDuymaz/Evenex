using Evenex.Application.Abstractions;
using Evenex.Application.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

/// <summary>
/// Etkinlik endpoint'leri: localhost:5177/api/event
/// Okuma herkese açık; oluşturma/güncelleme/silme için geçerli bir JWT gerekir ([Authorize]).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly ICurrentUserService _currentUser;

    public EventController(EventService eventService, ICurrentUserService currentUser)
    {
        _eventService = eventService;
        _currentUser = currentUser;
    }

    private string ClientIp => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll () => Ok(await _eventService.GetEventsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById (string id) => Ok(await _eventService.GetEventByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNewEvent([FromBody] CreateEventDto dto)
    {
        var created = await _eventService.CreateEventAsync(dto, _currentUser.Email, ClientIp);
        // Location header'ına HASH'li id yazılmalı (created.Id gerçek int id'dir, dışarı sızmamalı ve
        // /api/event/{id} adresi zaten hash bekliyor)
        return CreatedAtAction(nameof(GetEventById), new { id = created.HashedId }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent (string id, [FromBody] UpdateEventDto dto)
        => Ok(await _eventService.UpdateAsync(id, dto, _currentUser.Email, ClientIp));

    // Durum değiştirme: "bu etkinliği yayınla / iptal et" bir eylem olduğu için POST + eylem adı.
    // Kural ihlalinde (örn. iptal edilmiş etkinliği yayınlamak) 409 Conflict döner.
    [Authorize]
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishEvent (string id)
        => Ok(await _eventService.PublishAsync(id, _currentUser.Email, ClientIp));

    [Authorize]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelEvent (string id)
        => Ok(await _eventService.CancelAsync(id, _currentUser.Email, ClientIp));

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent (string id)
    {
        await _eventService.DeleteAsync(id);
        return NoContent();
    }
}
