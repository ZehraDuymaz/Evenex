namespace Evenex.Domain.Enums;

/// <summary>
/// Koltuğun anlık durumu: Available (boş) -> Held (birisi sepetinde tutuyor) -> Sold (satıldı).
/// </summary>
public enum SeatStatus
{
    Available,
    Held,
    Sold
}
