using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// İndirim sorguları. Ortak CRUD Repository&lt;Discount&gt;'den gelir.
/// DiscountService henüz yazılmadı; servis gelince buradaki hazır metotları kullanacak.
/// </summary>
public class DiscountRepository : Repository<Discount>, IDiscountRepository
{
    public DiscountRepository(AppDbContext db) : base(db) { }
}
