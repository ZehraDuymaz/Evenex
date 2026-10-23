using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Koltuk verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// </summary>
public interface ISeatRepository
{
    Task<Seat?> GetByIdAsync(int seatId);
}
