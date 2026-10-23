using AutoMapper;
using Evenex.Application.Common;
using Evenex.Domain.Entities;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;
    private readonly IMapper _mapper;

    public EventService (IEventRepository eventRepo, IVenueRepository venueRepo, IUnitOfWork unitOfWork, IHashids hashids, IMapper mapper)
    {
        _eventRepo = eventRepo;
        _venueRepo = venueRepo;
        _unitOfWork = unitOfWork;
        _hashids = hashids;
        _mapper = mapper;
    }

    public async Task<EventResponseDto> CreateEventAsync (CreateEventDto dto, string userEmail, string userIp)
    {
        // ArgumentException -> GlobalExceptionHandler bunu 400 Bad Request'e çevirir
        if (dto.EventDate < DateTime.UtcNow)
            throw new ArgumentException("Sadece gelecek tarihler için etkinlik oluşturulabilir.");

        int realVenueId = _hashids.DecodeId(dto.VenueId, "Mekan");

        // Mekân gerçekten var mı? Kontrol etmezsek SaveChanges'ta foreign key hatası (500) alırız.
        _ = await _venueRepo.GetVenueAsync(realVenueId)
            ?? throw new KeyNotFoundException($"Mekan bulunamadı: {dto.VenueId}");

        // Title, Description, EventDate AutoMapper ile doluyor; geri kalanları biz set ediyoruz.
        var newEvent = _mapper.Map<Event>(dto);
        newEvent.VenueId = realVenueId;
        newEvent.Status = "Draft"; // Yeni etkinlik taslak olarak başlar, yayınlama sonra eklenecek
        newEvent.CreatedUser = userEmail;
        newEvent.CreatedDate = DateTime.UtcNow;
        newEvent.CreatedIP = userIp;
        newEvent.IsDeleted = false;

        await _eventRepo.AddAsync(newEvent);
        // SaveChanges'tan sonra veritabanı newEvent.Id'yi doldurmuş olur
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(newEvent);
    }

    public async Task<EventResponseDto> GetEventByIdAsync (string id)
    {
        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var foundEvent = await _eventRepo.GetEventAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");

        return _mapper.Map<EventResponseDto>(foundEvent);
    }

    public async Task<IEnumerable<EventResponseDto>> GetEventsAsync ()
    {
        var rawEvents = await _eventRepo.GetAllEventsAsync();
        return _mapper.Map<List<EventResponseDto>>(rawEvents);
    }

    public async Task<EventResponseDto> UpdateAsync(string id, UpdateEventDto dto, string updatedByMail, string updatedByIp)
    {
        if (dto.EventDate < DateTime.UtcNow)
            throw new ArgumentException("Etkinlik tarihi geçmiş bir tarihe alınamaz.");

        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var existingEvent = await _eventRepo.GetEventAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");

        // Map(kaynak, hedef): yeni nesne oluşturmaz, var olan entity'nin alanlarını DTO'dakilerle günceller
        _mapper.Map(dto, existingEvent);
        existingEvent.ModifiedUser = updatedByMail;
        existingEvent.ModifiedDate = DateTime.UtcNow;
        existingEvent.ModifiedIP = updatedByIp;

        await _eventRepo.UpdateAsync(existingEvent);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EventResponseDto>(existingEvent);
    }

    public async Task DeleteAsync (string id)
    {
        int realEventId = _hashids.DecodeId(id, "Etkinlik");
        var existingEvent = await _eventRepo.GetEventAsync(realEventId)
            ?? throw new KeyNotFoundException($"Etkinlik bulunamadı: {id}");

        await _eventRepo.SoftDeleteAsync(existingEvent.Id);
        await _unitOfWork.SaveChangesAsync();
    }
}
