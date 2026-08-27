using System.Security.Claims;
using Evenex.Application.Venues;
using Evenex.Application.VenueSections;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;


namespace Evenex.Api.Controllers;

[ApiController]
[Route("api/[controller]")]


public class SectionController : ControllerBase
{
    private readonly SectionService _sectionService;
    private readonly IHashids _hashids;
    private readonly VenueService _venueService;
    public SectionController (SectionService sectionService, VenueService venueService, IHashids hashids)
    {
        _sectionService = sectionService;
        _hashids = hashids;
        _venueService = venueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVenues()
    {
        var venues = await _venueService.GetAllVenuesAsync();

        return Ok(venues);
    }

    [HttpPost] 
    public async Task<IActionResult> CreateNewSection ([FromBody] CreateSectionDto dto) 
    {   
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Yer Bölümü oluşturmak için ilk yer seçiniz.";
        string userIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        var newSectionId = await _sectionService.CreateVenueSectionAsync(dto, userEmail, userIP);
        
        string eId = _hashids.Encode(newSectionId);

        return Ok(new { Message = "Yeni Venue Section Oluşturuldu!", VenueId = newSectionId });
    }
 
}