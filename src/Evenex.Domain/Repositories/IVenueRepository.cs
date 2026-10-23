using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Mekân verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// </summary>
public interface IVenueRepository
{
    Task<Venue?> GetVenueAsync(int id);
    Task<IEnumerable<Venue>> GetAllAsync();
    Task AddAsync(Venue venue);
    Task UpdateAsync(Venue venue);
    Task SoftDeleteAsync(int id);
}