using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// Mekân bölümü sorguları. Ortak CRUD Repository&lt;Section&gt;'den gelir; burada sadece özel sorgular var.
/// </summary>
public class SectionRepository : Repository<Section>, ISectionRepository
{
    public SectionRepository (AppDbContext db) : base(db) { }

    // Bir mekâna ait, silinmemiş tüm bölümler.
    // _set = üst sınıftan gelen Sections tablosu (protected olduğu için burada erişebiliyoruz)
    public async Task<IEnumerable<Section>> GetSectionsByVenueIdAsync (int venueId)
    {
        return await _set.Where(s => s.VenueId == venueId && !s.IsDeleted).ToListAsync();
    }
}
