using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _db; 

    public EventRepository (AppDbContext db) => _db = db;

    public async Task<Event?> GetEventAsync(int id)
    {
        return await _db.Events.FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<IEnumerable<Event>> GetAllEventsAsync ()
    {
        return await _db.Events.Where(e => !e.IsDeleted).ToListAsync();
    }

    public async Task AddAsync (Event newEvent)
    {
        await _db.Events.AddAsync(newEvent);
    }
    public Task UpdateAsync (Event newEvent)
    {
        _db.Events.Update(newEvent);
        return Task.CompletedTask;
    }
    public async Task SoftDeleteAsync (int id)
    {
        var newEvent = await _db.Events.FirstOrDefaultAsync(v => v.Id == id);
        if (newEvent is null) return;

        newEvent.IsDeleted = true;
        newEvent.ModifiedDate = DateTime.UtcNow;
        _db.Events.Update(newEvent);
    }
}