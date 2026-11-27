using AutoMapper;
using Evenex.Domain.Entities;
using Evenex.Domain.Exceptions;
using Evenex.Domain.Repositories;

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
    private readonly IMapper _mapper;

    public EventSectionService(IEventSectionRepository eventSectionRepo, IEventRepository eventRepo, ISectionRepository sectionRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _eventSectionRepo = eventSectionRepo;
        _eventRepo = eventRepo;
        _sectionRepo = sectionRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Bir etkinliğin satış bölümleri (örn. etkinlik sayfasında "A Blok 500 TL" listesi)
    public async Task<IEnumerable<EventSectionResponseDto>> GetByEventAsync(int eventId)
    {
        var existingEvent = await GetEventAsync(eventId);
        var eventSections = await _eventSectionRepo.GetByEventIdAsync(existingEvent.Id);
        return _mapper.Map<List<EventSectionResponseDto>>(eventSections);
    }

    public async Task<EventSectionResponseDto> GetByIdAsync(int id)
        => _mapper.Map<EventSectionResponseDto>(await GetEventSectionAsync(id));

    public async Task<EventSectionResponseDto> CreateAsync(CreateEventSectionDto dto)
    {
        var existingEvent = await GetEventAsync(dto.EventId);
        // Yayındaki veya iptal edilmiş etkinliğe bölüm eklenemez (409)
        existingEvent.EnsureSectionsEditable();

        // dto.SectionId / dto.EventId gerçek int (JSON'dan gelirken [HashId] çözdü)
        var section = await _sectionRepo.GetByIdAsync(dto.SectionId)
            ?? throw new KeyNotFoundException("Bölüm bulunamadı.");

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

    public async Task<EventSectionResponseDto> UpdateAsync(int id, UpdateEventSectionDto dto)
    {
        var eventSection = await GetEventSectionAsync(id);
        var parentEvent = await LoadParentEventAsync(eventSection);
        parentEvent.EnsureSectionsEditable();

        eventSection.UpdatePricing(dto.BasePrice, dto.Capacity);

        await _eventSectionRepo.UpdateAsync(eventSection);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventSectionResponseDto>(eventSection);
    }

    public async Task DeleteAsync(int id)
    {
        var eventSection = await GetEventSectionAsync(id);
        var parentEvent = await LoadParentEventAsync(eventSection);
        parentEvent.EnsureSectionsEditable();

        await _eventSectionRepo.SoftDeleteAsync(eventSection);
        await _unitOfWork.SaveChangesAsync();
    }

    // ---------- Yardımcılar: "kaydı getir, yoksa 404" tekrarını tek yere topluyor ----------
    // id'ler gerçek int (hash'i model binder / JSON converter çözdü). Mesajlarda id yazmıyoruz ki sızmasın.

    private async Task<Event> GetEventAsync(int eventId)
        => await _eventRepo.GetByIdAsync(eventId) ?? throw new KeyNotFoundException("Etkinlik bulunamadı.");

    private async Task<EventSection> GetEventSectionAsync(int id)
        => await _eventSectionRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Etkinlik bölümü bulunamadı.");

    // Bölümün bağlı olduğu etkinlik (silinmişse bölüm de artık düzenlenemez)
    private async Task<Event> LoadParentEventAsync(EventSection eventSection)
        => await _eventRepo.GetByIdAsync(eventSection.EventId)
            ?? throw new KeyNotFoundException("Bu bölümün bağlı olduğu etkinlik bulunamadı.");
}
