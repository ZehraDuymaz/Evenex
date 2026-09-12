using System.Formats.Asn1;
using Evenex.Domain.Entities;
using Evenex.Domain.Enums;
using Evenex.Domain.Repositories;
using HashidsNet;
using Evenex.Application.Venues;

namespace Evenex.Application.VenueSections;

public class SectionService
{
    private readonly ISectionRepository _venueSectionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;
    private readonly IVenueRepository _venueRepo;
    public SectionService (IUnitOfWork unitOfWork, ISectionRepository sectionRepo, IVenueRepository venueRepo, IHashids hashids) 
    {
        _unitOfWork = unitOfWork;
        _venueSectionRepo = sectionRepo;
        _hashids = hashids;
        _venueRepo = venueRepo;
    }

    public async Task<IEnumerable<VenueResponseDto>> GetAllVenuesAsync()
    {
        
        var rawVenues = await _venueRepo.GetAllAsync();

        var responseDtos = rawVenues.Select(venue => new VenueResponseDto(
            Id: _hashids.Encode(venue.Id), 
            Name: venue.Name,
            Address: venue.Address
        )).ToList();

        return responseDtos;
    }

    public async Task<int> CreateVenueSectionAsync (CreateSectionDto dto, string userEmail, string userIp)
    {
        Console.WriteLine($"\n--- DEBUG: RECEIVED VENUE ID: '{dto.VenueId}' ---\n");
        var decodedIds = _hashids.Decode(dto.VenueId);
        if (decodedIds.Length == 0) throw new Exception ("Venue bulunamadı.");
        int realVenueId = decodedIds[0];
        var newSection = new Section ()
        {
            VenueId = realVenueId,
            Name = dto.Name,
            Type = dto.Type,
            CreatedUser = userEmail,
            CreatedDate = DateTime.UtcNow,
            CreatedIP = userIp,
            IsDeleted = false
        };

        await _venueSectionRepo.AddAsync(newSection);
        await _unitOfWork.SaveChangesAsync();

        return newSection.Id;
    }
}