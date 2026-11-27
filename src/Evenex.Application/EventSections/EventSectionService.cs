using AutoMapper;
using Evenex.Application.Common;
using Evenex.Domain.Entities;
using Evenex.Domain.Exceptions;
using Evenex.Domain.Repositories;
using HashidsNet;

namespace Evenex.Application.EventSections;

/// <summary>
/// Etkinlik satış bölümleri (EventSection) için iş kuralları: bir etkinlik, mekânının hangi bölümünü
/// hangi fiyat ve kontenjanla satacağını burada belirler.
///
/// Kuralların dağılımı:
///   EventSection entity'si -> değerlerin kendisi (fiyat negatif olamaz, kontenjan >= 1)
///   Event entity'si        -> etkinlik taslak değilse bölümler değiştirilemez (EnsureSectionsEditable)
///   Bu servis              -> başka kayıtlara bakmayı gerektirenler: etkinlik/bölüm var mı,
///                             bölüm etkinliğin mekânına mı ait, aynı bölüm zaten eklenmiş mi
/// </summary>
public class EventSectionService
{
    private readonly IEventSectionRepository _eventSectionRepo;
    private readonly IEventRepository _eventRepo;
    private readonly ISectionRepository _sectionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;
    private readonly IMapper _mapper;

    public EventSectionService(IEventSectionRepository eventSectionRepo, IEventRepository eventRepo, ISectionRepository sectionRepo,
        IUnitOfWork unitOfWork, IHashids hashids, IMapper mapper)
    {
        _eventSectionRepo = eventSectionRepo;
        _eventRepo = eventRepo;
        _sectionRepo = sectionRepo;
        _unitOfWork = unitOfWork;
        _hashids = hashids;
        _mapper = mapper;
    }

    // Bir etkinliğin satış bölümleri (örn. etkinlik sayfasında "A Blok 500 TL" listesi)
    public async Task<IEnumerable<EventSectionResponseDto>> GetByEventAsync(string eventId)
    {
        var existingEvent = await GetEventAsync(eventId);
        var eventSections = await _eventSectionRepo.GetByEventIdAsync(existingEvent.Id);
        return _mapper.Map<List<EventSectionResponseDto>>(eventSections);
    }

    public async Task<EventSectionResponseDto> GetByIdAsync(string id)
        => _mapper.Map<EventSectionResponseDto>(await GetEventSectionAsync(id));

    public async Task<EventSectionResponseDto> CreateAsync(CreateEventSectionDto dto)
    {
        var existingEvent = await GetEventAsync(dto.EventId);
        // Yayındaki veya iptal edilmiş etkinliğe bölüm eklenemez (409)
        existingEvent.EnsureSectionsEditable();

        int realSectionId = _hashids.DecodeId(dto.SectionId, "Bölüm");
        var section = await _sectionRepo.GetByIdAsync(realSectionId)
            ?? throw new KeyNotFoundException($"Bölüm bulunamadı: {dto.SectionId}");

        // Bölüm, etkinliğin yapıldığı mekâna ait olmalı: Zorlu'nun A Blok'u başka mekândaki etkinlikte satılamaz
        if (section.VenueId != existingEvent.VenueId)
            throw new ArgumentException("Bu bölüm, etkinliğin yapıldığı mekâna ait değil.");

        // Aynı bölüm iki kez eklenemez. Veritabanındaki unique index de engelliyor, ama o 500 hatası verirdi;
        // burada önceden bakıp anlaşılır bir 409 mesajı veriyoruz.
        if (await _eventSectionRepo.ExistsAsync(existingEvent.Id, section.Id))
            throw new DuplicateEventSectionException();

        // Fiyat/kontenjan kontrolü entity'nin Create metodunda (400). Created* alanlarını AuditInterceptor dolduruyor.
        var eventSection = EventSection.Create(existingEvent.Id, section.Id, dto.BasePrice, dto.Capacity);

        await _eventSectionRepo.AddAsync(eventSection);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventSectionResponseDto>(eventSection);
    }

    public async Task<EventSectionResponseDto> UpdateAsync(string id, UpdateEventSectionDto dto)
    {
        var eventSection = await GetEventSectionAsync(id);
        var parentEvent = await LoadParentEventAsync(eventSection);
        parentEvent.EnsureSectionsEditable();

        eventSection.UpdatePricing(dto.BasePrice, dto.Capacity);

        await _eventSectionRepo.UpdateAsync(eventSection);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventSectionResponseDto>(eventSection);
    }

    public async Task DeleteAsync(string id)
    {
        var eventSection = await GetEventSectionAsync(id);
        var parentEvent = await LoadParentEventAsync(eventSection);
        parentEvent.EnsureSectionsEditable();

        await _eventSectionRepo.SoftDeleteAsync(eventSection);
        await _unitOfWork.SaveChangesAsync();
    }

    // ---------- Yardımcılar: "id'yi çöz, kaydı getir, yoksa 404" tekrarını tek yere topluyor ----------

    private async Task<Event> GetEventAsync(string hashedEventId)
    {
        int realEventId = _hashids.DecodeId(hashedEventId, "Etkinlik");
        return await _eventRepo.GetByIdAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {hashedEventId}");
    }

    private async Task<EventSection> GetEventSectionAsync(string hashedId)
    {
        int realId = _hashids.DecodeId(hashedId, "Etkinlik bölümü");
        return await _eventSectionRepo.GetByIdAsync(realId)
            ?? throw new KeyNotFoundException($"Etkinlik bölümü bulunamadı: {hashedId}");
    }

    // Bölümün bağlı olduğu etkinlik (silinmişse bölüm de artık düzenlenemez)
    private async Task<Event> LoadParentEventAsync(EventSection eventSection)
        => await _eventRepo.GetByIdAsync(eventSection.EventId)
            ?? throw new KeyNotFoundException("Bu bölümün bağlı olduğu etkinlik bulunamadı.");
}
