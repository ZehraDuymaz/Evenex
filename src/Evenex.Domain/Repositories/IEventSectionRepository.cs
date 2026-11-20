using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Etkinlik bölümü verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD IRepository&lt;EventSection&gt;'den gelir.
/// Etkinlik veya bölüm getirmek gerekirse IEventRepository / ISectionRepository kullanılır;
/// her repository sadece kendi tablosundan sorumludur.
/// </summary>
public interface IEventSectionRepository : IRepository<EventSection>
{
    Task IncrementCapacityAsync(int eventSectionId);
}
