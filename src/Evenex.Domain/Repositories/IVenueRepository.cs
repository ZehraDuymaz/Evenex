using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

public interface IVenueRepository
{
    Task<Venue?> GetVenueAsync(int id);

    Task AddAsync(Venue venue);
}