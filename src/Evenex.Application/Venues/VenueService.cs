using AutoMapper;
using Evenex.Application.Common;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using HashidsNet;

namespace Evenex.Application.Venues;

/// <summary>
/// Mekân iş kuralları. Entity ↔ DTO dönüşümleri MappingProfile'daki kurallarla AutoMapper'a yaptırılır.
/// </summary>
public class VenueService
{
    private readonly IVenueRepository _venueRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;
    private readonly IMapper _mapper;

    public VenueService (IVenueRepository venueRepo, IUnitOfWork unitOfWork, IHashids hashids, IMapper mapper)
    {
        _venueRepo = venueRepo;
        _unitOfWork = unitOfWork;
        _hashids = hashids;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueResponseDto>> GetAllVenuesAsync ()
    {
        var rawVenues = await _venueRepo.GetAllAsync();
        return _mapper.Map<List<VenueResponseDto>>(rawVenues);
    }

    public async Task<VenueResponseDto> GetByIdAsync (string id)
    {
        int realVenueId = _hashids.DecodeId(id, "Mekan");
        var venue = await _venueRepo.GetByIdAsync(realVenueId)
            ?? throw new KeyNotFoundException($"Mekan bulunamadı: {id}");

        return _mapper.Map<VenueResponseDto>(venue);
    }

    public async Task<VenueResponseDto> CreateVenueAsync (CreateVenueDto dto, string userEmail, string userIp)
    {
        // Name ve Address AutoMapper ile doluyor; kim/ne zaman/nereden bilgisini biz ekliyoruz.
        var newVenue = _mapper.Map<Venue>(dto);
        newVenue.CreatedUser = userEmail;
        newVenue.CreatedDate = DateTime.UtcNow;
        newVenue.CreatedIP = userIp;
        newVenue.IsDeleted = false;

        await _venueRepo.AddAsync(newVenue);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VenueResponseDto>(newVenue);
    }

    public async Task<VenueResponseDto> UpdateAsync (string id, UpdateVenueDto dto, string updatedByMail, string updatedByIp)
    {
        int realVenueId = _hashids.DecodeId(id, "Mekan");
        var venue = await _venueRepo.GetByIdAsync(realVenueId)
            ?? throw new KeyNotFoundException($"Mekan bulunamadı: {id}");

        // Var olan entity'nin Name/Address alanlarını DTO'dakilerle üzerine yazar
        _mapper.Map(dto, venue);
        venue.ModifiedUser = updatedByMail;
        venue.ModifiedDate = DateTime.UtcNow;
        venue.ModifiedIP = updatedByIp;

        await _venueRepo.UpdateAsync(venue);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VenueResponseDto>(venue);
    }

    public async Task DeleteAsync (string id)
    {
        int realVenueId = _hashids.DecodeId(id, "Mekan");
        var venue = await _venueRepo.GetByIdAsync(realVenueId)
            ?? throw new KeyNotFoundException($"Mekan bulunamadı: {id}");

        await _venueRepo.SoftDeleteAsync(venue);
        await _unitOfWork.SaveChangesAsync();
    }
}
