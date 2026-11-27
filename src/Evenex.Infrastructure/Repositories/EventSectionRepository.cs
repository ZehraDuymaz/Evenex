using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// Etkinlik bölümü sorguları. Ortak CRUD Repository&lt;EventSection&gt;'den gelir.
/// </summary>
public class EventSectionRepository : Repository<EventSection>, IEventSectionRepository
{
    public EventSectionRepository (AppDbContext db) : base(db) { }

    public async Task<IEnumerable<EventSection>> GetByEventIdAsync(int eventId)
        => await _set.Where(es => es.EventId == eventId && !es.IsDeleted).ToListAsync();

    // AnyAsync: "en az bir tane var mı?" SQL'de EXISTS'e çevrilir; tüm kayıtları çekmeden cevap verir
    public Task<bool> ExistsAsync(int eventId, int sectionId)
        => _set.AnyAsync(es => es.EventId == eventId && es.SectionId == sectionId && !es.IsDeleted);

    // CountAsync: SQL'de COUNT(*); sadece sayı döner, kayıtları belleğe almaz
    public Task<int> CountByEventIdAsync(int eventId)
        => _set.CountAsync(es => es.EventId == eventId && !es.IsDeleted);

    // TODO: Rezervasyon iptal/expire olduğunda RemainingCapacity'yi geri artırmak için doldurulacak.
    public Task IncrementCapacityAsync(int eventSectionId)
    {
        return Task.CompletedTask;
    }
}
