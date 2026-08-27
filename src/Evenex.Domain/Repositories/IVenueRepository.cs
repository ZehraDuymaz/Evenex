using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

public interface IVenueRepository
{
    Task<Venue?> GetVenueAsync(int id);

    Task<IEnumerable<Venue>> GetAllAsync();

    Task AddAsync(Venue venue);
}