
using System.Security.Claims;
using Evenex.Application.Venues;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class VenueController : ControllerBase
{
    private readonly VenueService _venueService;
    private readonly IHashids _hashids;

    public VenueController (VenueService venueService, IHashids hashids)
    {
        _venueService = venueService;
        _hashids = hashids;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewVenue ([FromBody] CreateVenueDto dto)
    {
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Yer oluşturmak için hesap oluşturunuz.";

        string userIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        int newVenueId = await _venueService.CreateVenueAsync(dto, userEmail, userIP);
        
        string eId = _hashids.Encode(newVenueId);

        return Ok(new { Message = "Yeni Venue Oluşturuldu!", VenueID = eId });
    }
}