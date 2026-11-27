using Evenex.Domain.Common;
using Evenex.Domain.Enums;
using Evenex.Domain.Exceptions;

namespace Evenex.Domain.Entities;

/// <summary>
/// Etkinlik (konser, tiyatro, maç...). Her etkinlik tek bir mekânda (Venue) yapılır.
/// Bilet fiyatı ve kontenjan etkinliğin kendisinde değil, EventSection'larda tutulur:
/// aynı mekânın bölümleri farklı etkinliklerde farklı fiyata satılabilsin diye.
/// </summary>
public class Event : BaseEntity
{
    // Foreign key + navigation property ikilisi: VenueId veritabanındaki kolon,
    // Venue ise EF Core'un o id'den bulup doldurabildiği nesnenin kendisi.
    public int VenueId { get; set; }
    public Venue Venue { get; set; } = default!;

    public string Title { get; set; } = default!;
    // required: nesne oluşturulurken bu alan verilmek zorunda, yoksa derleme hatası
    public required string Description { get; set; }
    public DateTime EventDate { get; set; }
    // Yeni etkinlik her zaman taslak başlar.
    // private set: durumu dışarıdan kimse "event.Status = ..." diye değiştiremez; sadece aşağıdaki
    // Publish/Cancel metotları değiştirebilir. Böylece kurallar atlanamaz.
    // (EF Core private set'e veritabanından okurken yazabiliyor, sorun değil.)
    public EventStatus Status { get; private set; } = EventStatus.Draft;

    // Bu etkinlikte satışa açılan bölümler (fiyat + kontenjan bilgisiyle)
    public ICollection<EventSection> EventSections { get; set; } = new List<EventSection>();

    // ---------- Durum geçişleri (iş kuralları entity'nin içinde) ----------
    // Kurallar burada olunca hangi servis çağırırsa çağırsın aynı kurallar geçerli; tek bir yerde yazılı.

    /// <summary>
    /// Taslağı yayına alır. Sadece Draft durumundan ve en az bir satış bölümü varsa yapılabilir.
    /// activeSectionCount: bu etkinliğin silinmemiş EventSection sayısı. Sayıyı servis veritabanından
    /// alıp veriyor; böylece entity veritabanını bilmek zorunda kalmıyor, kural yine burada yazılı.
    /// </summary>
    public void Publish(int activeSectionCount)
    {
        if (Status != EventStatus.Draft)
            throw new InvalidEventStatusException(Status, "yayınlama");

        // Geçmiş tarihli bir etkinliği yayınlamanın anlamı yok
        if (EventDate < DateTime.UtcNow)
            throw new InvalidEventStatusException(Status, "geçmiş tarihli etkinliği yayınlama");

        // Satılacak hiçbir bölüm yoksa yayınlamak anlamsız: kimse bilet alamaz
        if (activeSectionCount == 0)
            throw new InvalidEventStatusException(Status, "satış bölümü olmadan yayınlama");

        Status = EventStatus.Published;
    }

    /// <summary>
    /// Satış bölümleri (EventSection) eklenip/değiştirilip/silinebilir mi? Sadece taslakken.
    /// Yayındayken bilet satılmış olabilir, iptal edilmişse zaten değişiklik anlamsız.
    /// </summary>
    public void EnsureSectionsEditable()
    {
        if (Status != EventStatus.Draft)
            throw new InvalidEventStatusException(Status, "satış bölümü değişikliği");
    }

    /// <summary>Etkinliği iptal eder. Zaten iptal edilmişse tekrar iptal edilemez.</summary>
    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
            throw new InvalidEventStatusException(Status, "iptal");

        Status = EventStatus.Cancelled;
    }
}
