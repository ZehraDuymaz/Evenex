using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Mekân bölümü verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD IRepository&lt;Section&gt;'den gelir; burada sadece bölüme özel sorgular var.
/// </summary>
public interface ISectionRepository : IRepository<Section>
{
    Task<IEnumerable<Section>> GetSectionsByVenueIdAsync (int venueId);
}
