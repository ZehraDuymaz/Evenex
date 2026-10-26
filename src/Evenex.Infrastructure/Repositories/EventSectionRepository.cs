using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// Etkinlik bölümü sorguları. Ortak CRUD Repository&lt;EventSection&gt;'den gelir.
/// </summary>
public class EventSectionRepository : Repository<EventSection>, IEventSectionRepository
{
    public EventSectionRepository (AppDbContext db) : base(db) { }

    // TODO: Rezervasyon iptal/expire olduğunda RemainingCapacity'yi geri artırmak için doldurulacak.
    public Task IncrementCapacityAsync(int eventSectionId)
    {
        return Task.CompletedTask;
    }
}
