using System.Formats.Asn1;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Application.Venues;

public class VenueService
{
    private readonly IVenueRepository _venueRepo;

    private readonly IUnitOfWork _unitOfWork;

    public VenueService (IVenueRepository venueRepo, IUnitOfWork unitOfWork)
    {
        _venueRepo = venueRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateVenueAsync (CreateVenueDto dto, string userEmail, string userIp)
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

        return newVenue.Id;
    }
}