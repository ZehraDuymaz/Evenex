using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Mekân verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD metotları IRepository&lt;Venue&gt;'den gelir; mekâna özel bir sorgu gerekirse buraya eklenir.
/// </summary>
public interface IVenueRepository : IRepository<Venue>
{
}
