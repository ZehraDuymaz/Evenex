using Evenex.Application.EventSections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

/// <summary>
/// Etkinlik satış bölümü endpoint'leri: localhost:5177/api/eventsection
/// Bir etkinliğin, mekânının hangi bölümünü hangi fiyat/kontenjanla sattığı.
/// Okuma herkese açık; ekleme/güncelleme/silme için token gerekir ve sadece etkinlik taslakken yapılabilir (yoksa 409).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventSectionController : ControllerBase
{
    private readonly EventSectionService _eventSectionService;

    public EventSectionController(EventSectionService eventSectionService)
    {
        _eventSectionService = eventSectionService;
    }

    // GET api/eventsection/event/{eventId} -> o etkinliğin tüm satış bölümleri
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetByEvent(string eventId)
        => Ok(await _eventSectionService.GetByEventAsync(eventId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
        => Ok(await _eventSectionService.GetByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventSectionDto dto)
    {
        var created = await _eventSectionService.CreateAsync(dto);
        // Location header'ında hash'li id (gerçek int id dışarı sızmasın)
        return CreatedAtAction(nameof(GetById), new { id = created.HashedId }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateEventSectionDto dto)
        => Ok(await _eventSectionService.UpdateAsync(id, dto));

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _eventSectionService.DeleteAsync(id);
        return NoContent();
    }
}
