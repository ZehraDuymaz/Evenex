using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Rezervasyon verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// </summary>
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id);
    Task AddAsync(Reservation reservation);
    Task<List<Reservation>> GetExpiredHeldAsync(DateTime asOf);
}
