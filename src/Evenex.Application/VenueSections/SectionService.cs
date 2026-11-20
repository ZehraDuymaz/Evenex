using AutoMapper;
using Evenex.Application.Common;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using HashidsNet;

namespace Evenex.Application.VenueSections;

/// <summary>
/// Mekân bölümleri (Section) için iş kuralları. Her bölüm bir mekâna (Venue) aittir.
/// </summary>
public class SectionService
{
    private readonly ISectionRepository _sectionRepo;
    private readonly IVenueRepository _venueRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;
    private readonly IMapper _mapper;

    public SectionService (IUnitOfWork unitOfWork, ISectionRepository sectionRepo, IVenueRepository venueRepo, IHashids hashids, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _sectionRepo = sectionRepo;
        _venueRepo = venueRepo;
        _hashids = hashids;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SectionResponseDto>> GetSectionsByVenueAsync (string venueId)
    {
        int realVenueId = _hashids.DecodeId(venueId, "Mekan");
        var sections = await _sectionRepo.GetSectionsByVenueIdAsync(realVenueId);
        return _mapper.Map<List<SectionResponseDto>>(sections);
    }

    public async Task<SectionResponseDto> CreateVenueSectionAsync (CreateSectionDto dto, string userEmail, string userIp)
    {
        int realVenueId = _hashids.DecodeId(dto.VenueId, "Mekan");

        // Bölüm eklenecek mekân gerçekten var mı (ve silinmemiş mi)?
        _ = await _venueRepo.GetByIdAsync(realVenueId)
            ?? throw new KeyNotFoundException($"Mekan bulunamadı: {dto.VenueId}");

        var newSection = _mapper.Map<Section>(dto);
        newSection.VenueId = realVenueId;
        newSection.CreatedUser = userEmail;
        newSection.CreatedDate = DateTime.UtcNow;
        newSection.CreatedIP = userIp;
        newSection.IsDeleted = false;

        await _sectionRepo.AddAsync(newSection);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SectionResponseDto>(newSection);
    }
}
