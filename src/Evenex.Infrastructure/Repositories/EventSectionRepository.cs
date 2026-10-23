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
        return await _db.Events.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }
    public async Task<Section?> GetSectionById (int id)
    {
        return await _db.Sections.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
    public async Task<EventSection?> GetByIdAsync(int id)
    {
        return await _db.EventSections.FirstOrDefaultAsync(es => es.Id == id && !es.IsDeleted);
    }

    // TODO: Rezervasyon iptal/expire olduğunda RemainingCapacity'yi geri artırmak için doldurulacak.
    public Task IncrementCapacityAsync(int eventSectionId)
    {
        return Task.CompletedTask;
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
        var eventSection = await _db.EventSections.FirstOrDefaultAsync(es => es.Id == id);
        if (eventSection is null) return;

        eventSection.IsDeleted = true;
        eventSection.ModifiedDate = DateTime.UtcNow;
        _db.EventSections.Update(eventSection);
    }
}
