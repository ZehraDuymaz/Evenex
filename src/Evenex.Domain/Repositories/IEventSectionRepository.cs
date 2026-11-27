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
    // Bir etkinliğin silinmemiş tüm satış bölümleri
    Task<IEnumerable<EventSection>> GetByEventIdAsync(int eventId);

    // Bu bölüm bu etkinliğe daha önce eklenmiş mi? (aynı bölümü iki kez eklemeyi engellemek için)
    Task<bool> ExistsAsync(int eventId, int sectionId);

    // Etkinliğin kaç aktif satış bölümü var? (bölümsüz etkinlik yayınlanamaz kuralı için)
    Task<int> CountByEventIdAsync(int eventId);

    Task IncrementCapacityAsync(int eventSectionId);
}
