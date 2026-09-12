using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

public interface IEventSectionRepository
{
    Task<Event?> GetEventById (int id);
    Task<Section?> GetSectionById (int id);
    Task<EventSection?> GetByIdAsync(int id);
    Task IncrementCapacityAsync(int eventSectionId);
    Task AddAsync (EventSection eventSection);
    Task UpdateAsync (EventSection eventSection);
    Task SoftDeleteAsync (int id);
}
