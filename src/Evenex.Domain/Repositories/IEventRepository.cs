using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Etkinlik verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// </summary>
public interface IEventRepository
{
    Task<Event?> GetEventAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task AddAsync(Event newEvent);
    Task UpdateAsync (Event newEvent);
    Task SoftDeleteAsync (int id);

}
