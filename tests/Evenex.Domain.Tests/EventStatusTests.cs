using Evenex.Domain.Entities;
using Evenex.Domain.Enums;
using Evenex.Domain.Exceptions;

namespace Evenex.Domain.Tests;

/// <summary>
/// Event'in durum geçiş kurallarını (Publish / Cancel) test eder.
/// Kurallar entity'nin içinde olduğu için veritabanı, servis veya API olmadan test edilebiliyor.
/// </summary>
public class EventStatusTests
{
    // Testlerde tekrar etmemek için: gelecek tarihli, taslak bir etkinlik
    private static Event NewDraftEvent(DateTime? date = null) => new()
    {
        Title = "Konser",
        Description = "Açıklama",
        EventDate = date ?? DateTime.UtcNow.AddDays(30)
    };

    [Fact]
    public void NewEvent_StartsAsDraft()
    {
        Assert.Equal(EventStatus.Draft, NewDraftEvent().Status);
    }

    [Fact]
    public void Publish_FromDraft_BecomesPublished()
    {
        var ev = NewDraftEvent();

        ev.Publish();

        Assert.Equal(EventStatus.Published, ev.Status);
    }

    [Fact]
    public void Publish_Twice_Throws()
    {
        var ev = NewDraftEvent();
        ev.Publish();

        Assert.Throws<InvalidEventStatusException>(() => ev.Publish());
    }

    [Fact]
    public void Publish_PastEvent_Throws()
    {
        var ev = NewDraftEvent(DateTime.UtcNow.AddDays(-1));

        Assert.Throws<InvalidEventStatusException>(() => ev.Publish());
        Assert.Equal(EventStatus.Draft, ev.Status); // hata olunca durum değişmemeli
    }

    [Fact]
    public void Cancel_FromDraftOrPublished_BecomesCancelled()
    {
        var draft = NewDraftEvent();
        var published = NewDraftEvent();
        published.Publish();

        draft.Cancel();
        published.Cancel();

        Assert.Equal(EventStatus.Cancelled, draft.Status);
        Assert.Equal(EventStatus.Cancelled, published.Status);
    }

    [Fact]
    public void Cancelled_CannotBePublishedOrCancelledAgain()
    {
        var ev = NewDraftEvent();
        ev.Cancel();

        Assert.Throws<InvalidEventStatusException>(() => ev.Publish());
        Assert.Throws<InvalidEventStatusException>(() => ev.Cancel());
    }
}
