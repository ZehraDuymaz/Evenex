using AutoMapper;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Application.Venues;

/// <summary>
/// Mekân iş kuralları. Entity ↔ DTO dönüşümleri MappingProfile'daki kurallarla AutoMapper'a yaptırılır.
/// </summary>
public class VenueService
{
    private readonly IVenueRepository _venueRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VenueService (IVenueRepository venueRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _venueRepo = venueRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VenueResponseDto>> GetAllVenuesAsync ()
    {
        var rawVenues = await _venueRepo.GetAllAsync();
        return _mapper.Map<List<VenueResponseDto>>(rawVenues);
    }

    // id artık gerçek int: URL'deki hash'i controller'daki [HashId] (model binder) çözdü
    public async Task<VenueResponseDto> GetByIdAsync (int id)
        => _mapper.Map<VenueResponseDto>(await GetVenueAsync(id));

    public async Task<VenueResponseDto> CreateVenueAsync (CreateVenueDto dto)
    {
        // Name ve Address AutoMapper ile doluyor. Kim/ne zaman/nereden (Created*) bilgisini
        // AuditInterceptor kaydederken otomatik ekliyor.
        var newVenue = _mapper.Map<Venue>(dto);

        await _venueRepo.AddAsync(newVenue);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VenueResponseDto>(newVenue);
    }

    public async Task<VenueResponseDto> UpdateAsync (int id, UpdateVenueDto dto)
    {
        var venue = await GetVenueAsync(id);

        // Var olan entity'nin Name/Address alanlarını DTO'dakilerle üzerine yazar
        _mapper.Map(dto, venue);

        await _venueRepo.UpdateAsync(venue);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VenueResponseDto>(venue);
    }

    public async Task DeleteAsync (int id)
    {
        var venue = await GetVenueAsync(id);

        await _venueRepo.SoftDeleteAsync(venue);
        await _unitOfWork.SaveChangesAsync();
    }

    // "Kaydı getir, yoksa 404" tek yerde. Mesajda id yazmıyoruz: bu gerçek int id, dışarı sızmamalı.
    private async Task<Venue> GetVenueAsync(int id)
        => await _venueRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Mekan bulunamadı.");
}
