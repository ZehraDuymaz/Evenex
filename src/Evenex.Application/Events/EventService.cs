using System.Text;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using HashidsNet;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Evenex.Application.Events;

public class EventService
{
    private readonly IEventRepository _eventRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashids _hashids;

    public EventService (IEventRepository eventRepo, IUnitOfWork unitOfWork, IHashids hashids)
    {
        _eventRepo = eventRepo;
        _unitOfWork = unitOfWork;
        _hashids = hashids;
    }
    public async Task<int> CreateEventAsync (CreateEventDto dto, string userEmail, string userIp)
    {
        if (dto.EventDate < DateTime.UtcNow)
        {
            throw new Exception("Sadece gelecek tarihler için etkinlik oluşturulabilir.");
        }
        var decodedIds = _hashids.Decode(dto.VenueId);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Event ID formatı.");
        int realVenueId = decodedIds[0];
        // unpack dto and build functionality
        var newEvent = new Event ()
        {
            VenueId = realVenueId,
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Status = "Draft", // todo: add a variable of sorf
            CreatedUser = userEmail,
            CreatedDate = DateTime.UtcNow,
            CreatedIP = userIp,
            IsDeleted = false,
        };

        await _eventRepo.AddAsync(newEvent);

        await _unitOfWork.SaveChangesAsync();

        return newEvent.Id;
    }
    public async Task<EventResponseDto> GetEventByIdAsync (string id)
    {
        var decodedIds = _hashids.Decode(id);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Etkinlik id'si");
        var realEventId = decodedIds[0];
        var newEvent = await _eventRepo.GetEventAsync(realEventId) ?? throw new Exception ("Etkinlik bulunamadı");
        string eId = _hashids.Encode(newEvent.Id);
        return new EventResponseDto(eId, newEvent.Title, newEvent.Description, newEvent.EventDate);
    }
    public async Task<IEnumerable<EventResponseDto>> GetEventsAsync ()
    {
        var rawEvents = await _eventRepo.GetAllEventsAsync();

        var responseDtos = rawEvents.Select(e => new EventResponseDto(
            Id: _hashids.Encode(e.Id),
            Title: e.Title,
            Description: e.Description,
            EventDate: e.EventDate
        )).ToList();
        return responseDtos;
    }

    public async Task<EventResponseDto> UpdateAsync(string id, EventResponseDto dto, string updatedByMail, string updatedByIp)
    {
        var decodedIds = _hashids.Decode(id);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Event ID formatı.");
        int realVenueId = decodedIds[0];
        var newEvent = await _eventRepo.GetEventAsync(realVenueId) ?? throw new Exception("Etkinlik bulunamadı: {id}");

        newEvent.Title = dto.Title;
        newEvent.Description = dto.Description;
        newEvent.EventDate = dto.EventDate;
        newEvent.ModifiedUser = updatedByMail;
        newEvent.ModifiedDate = DateTime.UtcNow;
        newEvent.ModifiedIP = updatedByIp;

        await _eventRepo.UpdateAsync(newEvent);
        await _unitOfWork.SaveChangesAsync();
        string eId = _hashids.Encode(newEvent.Id);
        return new EventResponseDto(eId, newEvent.Title, newEvent.Description, newEvent.EventDate);
    }

    public async Task DeleteAsync (string id)
    {
        var decodedIds = _hashids.Decode(id);
        if (decodedIds.Length == 0) throw new Exception("Geçersiz Event ID formatı.");
        int realVenueId = decodedIds[0];
        var venue = await _eventRepo.GetEventAsync(realVenueId) ?? throw new Exception("Etkinlik bulunamadı: {id}");
        await _eventRepo.SoftDeleteAsync(venue.Id);
        await _unitOfWork.SaveChangesAsync();       
    }

}