using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Etkinlik verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD metotları IRepository&lt;Event&gt;'den gelir.
/// </summary>
public interface IEventRepository : IRepository<Event>
{
}
