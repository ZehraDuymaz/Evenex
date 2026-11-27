using AutoMapper;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Application.VenueSections;

/// <summary>
/// Mekân bölümleri (Section) için iş kuralları. Her bölüm bir mekâna (Venue) aittir.
/// </summary>
public class SectionService
{
    private readonly ISectionRepository _sectionRepo;
    private readonly IVenueRepository _venueRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SectionService (IUnitOfWork unitOfWork, ISectionRepository sectionRepo, IVenueRepository venueRepo, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _sectionRepo = sectionRepo;
        _venueRepo = venueRepo;
        _mapper = mapper;
    }

    // venueId gerçek int: URL'deki hash'i controller'daki [HashId] çözdü
    public async Task<IEnumerable<SectionResponseDto>> GetSectionsByVenueAsync (int venueId)
    {
        var sections = await _sectionRepo.GetSectionsByVenueIdAsync(venueId);
        return _mapper.Map<List<SectionResponseDto>>(sections);
    }

    public async Task<SectionResponseDto> CreateVenueSectionAsync (CreateSectionDto dto)
    {
        // dto.VenueId gerçek int (JSON'dan gelirken [HashId] çözdü).
        // Bölüm eklenecek mekân gerçekten var mı (ve silinmemiş mi)?
        _ = await _venueRepo.GetByIdAsync(dto.VenueId)
            ?? throw new KeyNotFoundException("Mekan bulunamadı.");

        // Name, Type ve VenueId AutoMapper ile doluyor
        var newSection = _mapper.Map<Section>(dto);

        await _sectionRepo.AddAsync(newSection);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SectionResponseDto>(newSection);
    }
}
