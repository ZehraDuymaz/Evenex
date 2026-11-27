using Evenex.Domain.Common;

namespace Evenex.Domain.Entities;

/// <summary>
/// Bir etkinliğin, mekânın bir bölümünü (Section) hangi fiyat ve kontenjanla sattığı.
/// Örn. "Konser" etkinliğinde Zorlu PSM'nin A Blok'u: 500 TL, 200 kişi.
/// Fiyat Section'da değil burada, çünkü aynı A Blok farklı etkinliklerde farklı fiyata satılabilir.
///
/// Değerler dışarıdan doğrudan set EDİLEMEZ (private set); sadece Create ve UpdatePricing ile değişir.
/// Böylece "negatif fiyat" veya "satılandan az kontenjan" gibi geçersiz durumlar hiç oluşamaz
/// (Event.Status için yaptığımızın aynısı).
/// </summary>
public class EventSection : BaseEntity
{
    public int EventId { get; private set; }
    public Event Event { get; set; } = default!;

    public int SectionId { get; private set; }
    public Section Section { get; set; } = default!;

    public decimal BasePrice { get; private set; }
    public int Capacity { get; private set; }

    // Henüz satılmamış yer sayısı. Satış/rezervasyon geldiğinde azalacak.
    public int RemainingCapacity { get; private set; }

    // Eşzamanlılık (concurrency) kontrolü: SQL Server bu değeri her güncellemede KENDİSİ değiştirir
    // (AppDbContext'te IsRowVersion). İki kişi aynı anda son bileti almaya çalışırsa, ikincisinin
    // elindeki RowVersion artık eski olduğu için kaydı reddedilir; aynı yer iki kere satılamaz.
    public byte[] RowVersion { get; private set; } = default!;

    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    // EF Core veritabanından okurken nesneyi oluşturmak için parametresiz constructor ister.
    // private: kod içinde kimse "new EventSection()" diye boş/geçersiz bir nesne oluşturamasın.
    private EventSection() { }

    /// <summary>
    /// Yeni satış bölümü oluşturur. Henüz hiçbir şey satılmadığı için kalan kontenjan = toplam kontenjan.
    /// Burada sadece DEĞERLERİN kendisi kontrol edilir; "etkinlik taslak mı", "bölüm aynı mekânda mı"
    /// gibi başka kayıtlara bakmayı gerektiren kurallar serviste.
    /// </summary>
    public static EventSection Create(int eventId, int sectionId, decimal basePrice, int capacity)
    {
        Validate(basePrice, capacity);
        return new EventSection
        {
            EventId = eventId,
            SectionId = sectionId,
            BasePrice = basePrice,
            Capacity = capacity,
            RemainingCapacity = capacity
        };
    }

    /// <summary>Fiyat ve kontenjanı günceller. Kontenjan, satılmış yer sayısının altına inemez.</summary>
    public void UpdatePricing(decimal basePrice, int capacity)
    {
        Validate(basePrice, capacity);

        int sold = Capacity - RemainingCapacity;
        if (capacity < sold)
            throw new ArgumentException($"Kontenjan satılmış yer sayısının ({sold}) altına indirilemez.");

        BasePrice = basePrice;
        Capacity = capacity;
        RemainingCapacity = capacity - sold;
    }

    // ArgumentException -> GlobalExceptionHandler 400 Bad Request döner (kullanıcı hatalı değer gönderdi)
    private static void Validate(decimal basePrice, int capacity)
    {
        if (basePrice < 0)
            throw new ArgumentException("Fiyat negatif olamaz.");
        if (capacity <= 0)
            throw new ArgumentException("Kontenjan en az 1 olmalı.");
    }
}
