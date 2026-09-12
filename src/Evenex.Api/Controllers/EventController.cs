using System.Security.Claims;
using Evenex.Application.Events;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

//localhost:xxxx/api/...
[ApiController]
[Route("api/[controller]")]

public class EventController : ControllerBase
{
    private readonly EventService _eventService;
    private readonly IHashids _hashids;
    
    public EventController(EventService eventService, IHashids hashids)
    {
        _eventService = eventService;
        _hashids = hashids;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll () => Ok(await _eventService.GetEventsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById (string id)
    {
        try
        {
            return Ok(await _eventService.GetEventByIdAsync(id));
        }
        catch (KeyNotFoundException exErr)
        {
            return NotFound(exErr.Message);
        } 
    }

    [HttpPost]
    // creating the actions for the event 
    public async Task<IActionResult> CreateNewEvent([FromBody] CreateEventDto dto)
    {   
        
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Etkinlik oluşturmak için hesap oluştunuz.";

        string userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        int newEventId = await _eventService.CreateEventAsync(dto, userEmail, userIp);

        string eId = _hashids.Encode(newEventId);

        return Ok(new { Message = "Yeni Event Oluşturuldu!", EventId = eId });
    }
    

}

