using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

public interface IEventRepository
{
    Task<Event?> GetEventAsync(int id);
    Task AddAsync(Event newEvent);
    

}
