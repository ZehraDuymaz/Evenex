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
            Name: venue.Name,
            Address: venue.Address
        )).ToList();

        return responseDtos;
    }

    public async Task<VenueResponseDto> GetByIdAsync (string id)
    {
        var decodedIds = _hashids.Decode(id);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Venue ID formatı.");
        int realVenueId = decodedIds[0];
        var venue = await _venueRepo.GetVenueAsync(realVenueId) ?? throw new Exception("Mekan bulunamadı: {id}");
        string eId = _hashids.Encode(venue.Id);
        return new VenueResponseDto(eId, venue.Name, venue.Address);
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

    public async Task<VenueResponseDto> UpdateAsync (string id, UpdateVenueDto dto, string updatedByMail, string updatedByIp) 
    {   
        var decodedIds = _hashids.Decode(id);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Venue ID formatı.");
        int realVenueId = decodedIds[0];
        var venue = await _venueRepo.GetVenueAsync(realVenueId) ?? throw new Exception("Mekan bulunamadı: {id}");

        venue.Name = dto.Name;
        venue.Address = dto.Address;
        venue.ModifiedUser = updatedByMail;
        venue.ModifiedDate = DateTime.UtcNow;
        venue.ModifiedIP = updatedByIp;

        await _venueRepo.UpdateAsync(venue);
        await _unitOfWork.SaveChangesAsync();

        string eId = _hashids.Encode(venue.Id);

        return new VenueResponseDto(eId, venue.Name, venue.Address);
    }
    public async Task DeleteAsync (string id)
    {
        var decodedIds = _hashids.Decode(id);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Venue ID formatı.");
        int realVenueId = decodedIds[0];
        var venue = await _venueRepo.GetVenueAsync(realVenueId) ?? throw new Exception("Mekan bulunamadı: {id}");
        await _venueRepo.SoftDeleteAsync(venue.Id);
        await _unitOfWork.SaveChangesAsync();       
    }
}