using System.Runtime.CompilerServices;
using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _db;
    public VenueRepository (AppDbContext db) => _db = db;
    public async Task<Venue?> GetVenueAsync(int id)
    {
        return await _db.Venues.FirstOrDefaultAsync(e => e.Id == id);
    } 
    public async Task<IEnumerable<Venue>> GetAllAsync ()
    {
        return await _db.Venues.Where(v => !v.IsDeleted).ToListAsync();
    }
    public Task<Venue?> GetByIdAsync (int id) => _db.Venues.FirstOrDefaultAsync(v => v.Id == id);
    public async Task AddAsync (Venue venue)
    {
        await _db.Venues.AddAsync(venue);
    }
    public Task UpdateAsync (Venue venue)
    {
        _db.Venues.Update(venue);
        return Task.CompletedTask;
    }
    public async Task SoftDeleteAsync (int id)
    {
        var venue = await _db.Venues.FirstOrDefaultAsync(v => v.Id == id);
        if (venue is null) return;

        venue.IsDeleted = true;
        venue.ModifiedDate = DateTime.UtcNow;
        _db.Venues.Update(venue);
    }
}
