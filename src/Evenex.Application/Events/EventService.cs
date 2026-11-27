using AutoMapper;
using Evenex.Application.Common;
using Evenex.Domain.Entities;
using Evenex.Domain.Enums;
using Evenex.Domain.Exceptions;
using Evenex.Domain.Repositories;
using HashidsNet;

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
    private readonly IHashids _hashids;
    private readonly IMapper _mapper;

    public EventService (IEventRepository eventRepo, IVenueRepository venueRepo, IEventSectionRepository eventSectionRepo, IUnitOfWork unitOfWork, IHashids hashids, IMapper mapper)
    {
        _eventRepo = eventRepo;
        _venueRepo = venueRepo;
        _eventSectionRepo = eventSectionRepo;
        _unitOfWork = unitOfWork;
        _hashids = hashids;
        _mapper = mapper;
    }

    public async Task<EventResponseDto> CreateEventAsync (CreateEventDto dto)
    {
        // ArgumentException -> GlobalExceptionHandler bunu 400 Bad Request'e çevirir
        if (dto.EventDate < DateTime.UtcNow)
            throw new ArgumentException("Sadece gelecek tarihler için etkinlik oluşturulabilir.");

        int realVenueId = _hashids.DecodeId(dto.VenueId, "Mekan");

        // Mekân gerçekten var mı? Kontrol etmezsek SaveChanges'ta foreign key hatası (500) alırız.
        _ = await _venueRepo.GetByIdAsync(realVenueId)
            ?? throw new KeyNotFoundException($"Mekan bulunamadı: {dto.VenueId}");

        // Title, Description, EventDate AutoMapper ile doluyor; VenueId'yi biz set ediyoruz.
        // Created* alanlarını AuditInterceptor kaydederken otomatik dolduruyor.
        var newEvent = _mapper.Map<Event>(dto);
        newEvent.VenueId = realVenueId;
        // Status burada set edilmiyor: Event entity'si her zaman Draft başlıyor (bkz. Event.Status)

        await _eventRepo.AddAsync(newEvent);
        // SaveChanges'tan sonra veritabanı newEvent.Id'yi doldurmuş olur
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(newEvent);
    }

    public async Task<EventResponseDto> GetEventByIdAsync (string id)
    {
        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var foundEvent = await _eventRepo.GetByIdAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");
            
        return _mapper.Map<EventResponseDto>(foundEvent);
    }

    public async Task<IEnumerable<EventResponseDto>> GetEventsAsync ()
    {
        var rawEvents = await _eventRepo.GetAllAsync();
        return _mapper.Map<List<EventResponseDto>>(rawEvents);
    }

    public async Task<EventResponseDto> UpdateAsync(string id, UpdateEventDto dto)
    {
        if (dto.EventDate < DateTime.UtcNow)
            throw new ArgumentException("Etkinlik tarihi geçmiş bir tarihe alınamaz.");

        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var existingEvent = await _eventRepo.GetByIdAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");

        // İptal edilmiş etkinlik artık düzenlenemez (409 Conflict)
        if (existingEvent.Status == EventStatus.Cancelled)
            throw new InvalidEventStatusException(existingEvent.Status, "güncelleme");

        // Map(kaynak, hedef): yeni nesne oluşturmaz, var olan entity'nin alanlarını DTO'dakilerle günceller
        _mapper.Map(dto, existingEvent);

        await _eventRepo.UpdateAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(existingEvent);
    }

    public async Task DeleteAsync (string id)
    {
        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var existingEvent = await _eventRepo.GetByIdAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");

        await _eventRepo.SoftDeleteAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();
    }

    // ---------- Durum değiştirme (yayınla / iptal et) ----------
    // Kuralların kendisi Event entity'sinde (Publish/Cancel). Servisin işi sadece: id'yi çöz, kaydı getir,
    // entity'ye "yayınla" de, kaydet (kim/ne zaman bilgisini AuditInterceptor yazıyor). Kural ihlalinde entity 409'a dönen hata fırlatır.

    // Yayınlamak için etkinliğin kaç aktif satış bölümü olduğunu veritabanından sorup entity'ye veriyoruz;
    // "bölüm yoksa yayınlanamaz" kuralının kendisi Event.Publish içinde.
    public Task<EventResponseDto> PublishAsync(string id)
        => ChangeStatusAsync(id, async e => e.Publish(await _eventSectionRepo.CountByEventIdAsync(e.Id)));

    public Task<EventResponseDto> CancelAsync(string id)
        => ChangeStatusAsync(id, e => { e.Cancel(); return Task.CompletedTask; });

    // Yayınla ve iptal et aynı adımlardan geçiyor; sadece ortadaki işlem farklı.
    // Func<Event, Task> = "bir Event alan ve beklenebilir (async) bir iş yapan fonksiyon".
    // Async olmasının sebebi: yayınlamadan önce bölüm sayısını veritabanından sormamız gerekiyor.
    private async Task<EventResponseDto> ChangeStatusAsync(string id, Func<Event, Task> transition)
    {
        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var existingEvent = await _eventRepo.GetByIdAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");

        await transition(existingEvent);

        await _eventRepo.UpdateAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(existingEvent);
    }
}
