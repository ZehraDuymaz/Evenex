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
        var venues = await _venueService.GetAllVenuesAsync();
        return Ok(venues);
    }
}