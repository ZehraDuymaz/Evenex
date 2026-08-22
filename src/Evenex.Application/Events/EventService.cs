using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Application.Events;

public class EventService
{
    private readonly IEventRepository _eventRepo;
    private readonly IUnitOfWork _unitOfWork;

    public EventService (IEventRepository eventRepo, IUnitOfWork unitOfWork)
    {
        _eventRepo = eventRepo;
        _unitOfWork = unitOfWork;
    }

    
    public async Task<int> CreateEventAsync (CreateEventDto dto, string userEmail, string userIp)
    {
        if (dto.EventDate < DateTime.UtcNow)
        {
            throw new Exception("Sadece gelecek tarihler için etkinlik oluşturulabilir.");
        }

        // unpack dto and build functionality
        var newEvent = new Event ()
        {
            VenueId = dto.VenueId,
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
    
}