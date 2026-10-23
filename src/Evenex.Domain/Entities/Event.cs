using Evenex.Domain.Common;

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
    // Draft (taslak) -> yayınlanınca değişecek. İleride enum'a çevrilebilir.
    public string Status { get; set; } = "Draft";

    // Bu etkinlikte satışa açılan bölümler (fiyat + kontenjan bilgisiyle)
    public ICollection<EventSection> EventSections { get; set; } = new List<EventSection>();
}
