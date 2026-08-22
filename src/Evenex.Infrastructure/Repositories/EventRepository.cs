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

    public async Task AddAsync (Event newEvent)
    {
        await _db.Events.AddAsync(newEvent);
    }
}