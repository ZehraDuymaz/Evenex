using AutoMapper;
using Evenex.Domain.Entities;
using Evenex.Domain.Enums;
using Evenex.Domain.Exceptions;
using Evenex.Domain.Repositories;

namespace Evenex.Application.Events;

/// <summary>
/// Etkinlik iş kuralları. Controller ile repository arasında durur:
/// id'leri çözer, kuralları kontrol eder, entity ↔ DTO dönüşümünü AutoMapper'a yaptırır.
/// </summary>
public class EventService
{
    private readonly IEventRepository _eventRepo;
    private readonly IVenueRepository _venueRepo;
    private readonly IEventSectionRepository _eventSectionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService (IEventRepository eventRepo, IVenueRepository venueRepo, IEventSectionRepository eventSectionRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _eventRepo = eventRepo;
        _venueRepo = venueRepo;
        _eventSectionRepo = eventSectionRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EventResponseDto> CreateEventAsync (CreateEventDto dto)
    {
        // ArgumentException -> GlobalExceptionHandler bunu 400 Bad Request'e çevirir
        if (dto.EventDate < DateTime.UtcNow)
            throw new ArgumentException("Sadece gelecek tarihler için etkinlik oluşturulabilir.");

        // dto.VenueId gerçek int (JSON'dan gelirken [HashId] çözdü).
        // Mekân gerçekten var mı? Kontrol etmezsek SaveChanges'ta foreign key hatası (500) alırız.
        _ = await _venueRepo.GetByIdAsync(dto.VenueId)
            ?? throw new KeyNotFoundException("Mekan bulunamadı.");

        // Title, Description, EventDate ve VenueId AutoMapper ile doluyor.
        // Created* alanlarını AuditInterceptor kaydederken otomatik dolduruyor.
        var newEvent = _mapper.Map<Event>(dto);
        // Status burada set edilmiyor: Event entity'si her zaman Draft başlıyor (bkz. Event.Status)

        await _eventRepo.AddAsync(newEvent);
        // SaveChanges'tan sonra veritabanı newEvent.Id'yi doldurmuş olur
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(newEvent);
    }

    public async Task<EventResponseDto> GetEventByIdAsync (int id)
    {
        var foundEvent = await GetEventAsync(id);
        return _mapper.Map<EventResponseDto>(foundEvent);
    }

    public async Task<IEnumerable<EventResponseDto>> GetEventsAsync ()
    {
        var rawEvents = await _eventRepo.GetAllAsync();
        return _mapper.Map<List<EventResponseDto>>(rawEvents);
    }

    public async Task<EventResponseDto> UpdateAsync(int id, UpdateEventDto dto)
    {
        if (dto.EventDate < DateTime.UtcNow)
            throw new ArgumentException("Etkinlik tarihi geçmiş bir tarihe alınamaz.");

        var existingEvent = await GetEventAsync(id);

        // İptal edilmiş etkinlik artık düzenlenemez (409 Conflict)
        if (existingEvent.Status == EventStatus.Cancelled)
            throw new InvalidEventStatusException(existingEvent.Status, "güncelleme");

        // Map(kaynak, hedef): yeni nesne oluşturmaz, var olan entity'nin alanlarını DTO'dakilerle günceller
        _mapper.Map(dto, existingEvent);

        await _eventRepo.UpdateAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(existingEvent);
    }

    public async Task DeleteAsync (int id)
    {
        var existingEvent = await GetEventAsync(id);

        await _eventRepo.SoftDeleteAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();
    }

    // ---------- Durum değiştirme (yayınla / iptal et) ----------
    // Kuralların kendisi Event entity'sinde (Publish/Cancel). Servisin işi sadece: kaydı getir,
    // entity'ye "yayınla" de, kaydet (kim/ne zaman bilgisini AuditInterceptor yazıyor). Kural ihlalinde entity 409'a dönen hata fırlatır.

    // Yayınlamak için etkinliğin kaç aktif satış bölümü olduğunu veritabanından sorup entity'ye veriyoruz;
    // "bölüm yoksa yayınlanamaz" kuralının kendisi Event.Publish içinde.
    public Task<EventResponseDto> PublishAsync(int id)
        => ChangeStatusAsync(id, async e => e.Publish(await _eventSectionRepo.CountByEventIdAsync(e.Id)));

    public Task<EventResponseDto> CancelAsync(int id)
        => ChangeStatusAsync(id, e => { e.Cancel(); return Task.CompletedTask; });

    // Yayınla ve iptal et aynı adımlardan geçiyor; sadece ortadaki işlem farklı.
    // Func<Event, Task> = "bir Event alan ve beklenebilir (async) bir iş yapan fonksiyon".
    // Async olmasının sebebi: yayınlamadan önce bölüm sayısını veritabanından sormamız gerekiyor.
    private async Task<EventResponseDto> ChangeStatusAsync(int id, Func<Event, Task> transition)
    {
        var existingEvent = await GetEventAsync(id);

        await transition(existingEvent);

        await _eventRepo.UpdateAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(existingEvent);
    }

    // "Kaydı getir, yoksa 404" tek yerde. id gerçek int (URL'deki hash'i [HashId] çözdü);
    // mesajda yazmıyoruz ki gerçek id dışarı sızmasın.
    private async Task<Event> GetEventAsync(int id)
        => await _eventRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Etkinlik bulunamadı.");
}
