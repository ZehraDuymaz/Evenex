using Evenex.Common.Hashing;
using Evenex.Application.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

/// <summary>
/// Etkinlik endpoint'leri: localhost:5177/api/event
/// Okuma herkese açık; oluşturma/güncelleme/silme için geçerli bir JWT gerekir ([Authorize]).
/// [HashId] int id: URL'deki hash ("k1JppaeX") controller'a gelmeden gerçek int'e çevrilir (HashIdModelBinder);
/// uydurma hash 404 döner. Servisler hash diye bir şey görmez.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly EventService _eventService;

    public EventController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll () => Ok(await _eventService.GetEventsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById ([HashId] int id) => Ok(await _eventService.GetEventByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNewEvent([FromBody] CreateEventDto dto)
    {
        var created = await _eventService.CreateEventAsync(dto);
        // Location header'ına HASH'li id yazılmalı (created.Id gerçek int id'dir, dışarı sızmamalı ve
        // /api/event/{id} adresi zaten hash bekliyor)
        return CreatedAtAction(nameof(GetEventById), new { id = created.HashedId }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent ([HashId] int id, [FromBody] UpdateEventDto dto)
        => Ok(await _eventService.UpdateAsync(id, dto));

    // Durum değiştirme: "bu etkinliği yayınla / iptal et" bir eylem olduğu için POST + eylem adı.
    // Kural ihlalinde (örn. iptal edilmiş etkinliği yayınlamak) 409 Conflict döner.
    [Authorize]
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> PublishEvent ([HashId] int id)
        => Ok(await _eventService.PublishAsync(id));

    [Authorize]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelEvent ([HashId] int id)
        => Ok(await _eventService.CancelAsync(id));

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent ([HashId] int id)
    {
        await _eventService.DeleteAsync(id);
        return NoContent();
    }
}
