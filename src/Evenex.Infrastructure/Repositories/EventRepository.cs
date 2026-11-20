using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// Etkinlik sorguları. Tüm CRUD kodu Repository&lt;Event&gt;'den miras gelir;
/// silinmiş etkinlikler orada "yok" sayılır, id ile de getirilemez.
/// </summary>
public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext db) : base(db) { }
}
