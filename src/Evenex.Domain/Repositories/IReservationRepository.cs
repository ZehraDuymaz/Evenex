using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Rezervasyon verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD IRepository&lt;Reservation&gt;'den gelir; burada sadece rezervasyona özel sorgular var.
/// </summary>
public interface IReservationRepository : IRepository<Reservation>
{
    // Süresi (asOf anına göre) dolmuş ama hâlâ "Held" durumundaki rezervasyonlar
    Task<List<Reservation>> GetExpiredHeldAsync(DateTime asOf);
}
