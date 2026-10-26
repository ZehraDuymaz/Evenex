using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// İndirim verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD IRepository&lt;Discount&gt;'den gelir; indirime özel sorgular (örn. koda göre bul) buraya eklenecek.
/// </summary>
public interface IDiscountRepository : IRepository<Discount>
{
}
