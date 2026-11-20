using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Koltuk verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD IRepository&lt;Seat&gt;'den gelir.
/// </summary>
public interface ISeatRepository : IRepository<Seat>
{
}
