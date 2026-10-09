using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

public class EventSectionRepository : IEventSectionRepository
{
    private readonly AppDbContext _db;

    public EventSectionRepository (AppDbContext db) => _db = db;

    public async Task<Event?> GetEventById (int id) 
    {
        return await _db.Events.FirstOrDefaultAsync(e => e.Id == id);
    } 
    public async Task<Section?> GetSectionById (int id) 
    {
        return await _db.Sections.FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<EventSection?> GetByIdAsync(int id) 
    {
        return await _db.EventSections.FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task IncrementCapacityAsync(int eventSectionId) 
    {

    }
    public async Task AddAsync (EventSection eventSection) 
    {
        await _db.EventSections.AddAsync(eventSection);
    }
    public Task UpdateAsync (EventSection eventSection) 
    {
        _db.EventSections.Update(eventSection);
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