using Evenex.Domain.Entities;

namespace Evenex.Domain.Tests;

/// <summary>
/// EventSection'ın kendi değer kurallarını test eder (fiyat, kontenjan, kalan kontenjan).
/// "Etkinlik taslak mı" kuralı Event'te (EventStatusTests), "aynı mekân mı / zaten eklenmiş mi" serviste.
/// </summary>
public class EventSectionTests
{
    [Fact]
    public void Create_SetsRemainingCapacityToCapacity()
    {
        var es = EventSection.Create(eventId: 1, sectionId: 2, basePrice: 500m, capacity: 200);

        Assert.Equal(500m, es.BasePrice);
        Assert.Equal(200, es.Capacity);
        Assert.Equal(200, es.RemainingCapacity); // henüz hiçbir şey satılmadı
    }

    [Fact]
    public void Create_FreeEvent_IsAllowed()
    {
        // Ücretsiz etkinlik olabilir: fiyat 0 geçerli
        var es = EventSection.Create(1, 2, basePrice: 0m, capacity: 10);

        Assert.Equal(0m, es.BasePrice);
    }

    [Theory]
    [InlineData(-1, 10)]   // negatif fiyat
    [InlineData(100, 0)]   // sıfır kontenjan
    [InlineData(100, -5)]  // negatif kontenjan
    public void Create_InvalidValues_Throws(decimal price, int capacity)
    {
        Assert.Throws<ArgumentException>(() => EventSection.Create(1, 2, price, capacity));
    }

    [Fact]
    public void UpdatePricing_ChangesPriceAndCapacity()
    {
        var es = EventSection.Create(1, 2, 500m, 200);

        es.UpdatePricing(450m, 250);

        Assert.Equal(450m, es.BasePrice);
        Assert.Equal(250, es.Capacity);
        Assert.Equal(250, es.RemainingCapacity);
    }

    [Fact]
    public void UpdatePricing_InvalidValues_Throws()
    {
        var es = EventSection.Create(1, 2, 500m, 200);

        Assert.Throws<ArgumentException>(() => es.UpdatePricing(-10m, 200));
        Assert.Throws<ArgumentException>(() => es.UpdatePricing(500m, 0));
    }
}
