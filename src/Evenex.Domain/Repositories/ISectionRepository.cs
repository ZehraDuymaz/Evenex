using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;


/// <summary>
/// Mekân bölümü verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// </summary>
public interface ISectionRepository
{
    Task<Section?> GetSectionAsync (int id);

    Task<IEnumerable<Section>> GetSectionsByVenueIdAsync (int venueId);

    Task AddAsync (Section section);
}
