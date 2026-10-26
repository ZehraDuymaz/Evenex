using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// Mekân sorguları. Tüm CRUD kodu Repository&lt;Venue&gt;'den miras gelir.
/// Not: AppDbContext'teki global query filter sayesinde silinmiş mekânlar hiçbir sorguda gelmez.
/// </summary>
public class VenueRepository : Repository<Venue>, IVenueRepository
{
    // base(db): gelen AppDbContext'i üst sınıfın (Repository<Venue>) constructor'ına iletir
    public VenueRepository(AppDbContext db) : base(db) { }
}
