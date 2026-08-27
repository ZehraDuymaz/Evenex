using Evenex.Application.VenueSections;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using HashidsNet;

namespace Evenex.Application.Venues;

public class VenueService
{
    private readonly IVenueRepository _venueRepo;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;

    public VenueService (IVenueRepository venueRepo, IUnitOfWork unitOfWork, IHashids hashids)
    {
        _venueRepo = venueRepo;
        _unitOfWork = unitOfWork;
        _hashids = hashids;
    }

    public async Task<IEnumerable<VenueResponseDto>> GetAllVenuesAsync ()
    {
        var rawVenues = await _venueRepo.GetAllAsync();

        var responseDtos = rawVenues.Select(venue => new VenueResponseDto(
            Id: _hashids.Encode(venue.Id),
            Name: venue.Name
        )).ToList();

        return responseDtos;
    }

    public async Task<string> CreateVenueAsync (CreateVenueDto dto, string userEmail, string userIp)
    {
        var newVenue = new Venue ()
        {
            Name = dto.Name,
            Address = dto.Address,
            CreatedUser = userEmail,
            CreatedDate = DateTime.UtcNow,
            CreatedIP = userIp,
            IsDeleted = false,
        };
        
        await _venueRepo.AddAsync(newVenue);
        await _unitOfWork.SaveChangesAsync();

        return _hashids.Encode(newVenue.Id);
    }
}