namespace Evenex.Domain.Enums;

/// <summary>
/// Rezervasyonun yaşam döngüsü:
/// Held (5 dk tutuluyor) -> Confirmed (ödendi) ya da Expired (süre doldu) / Cancelled (iptal).
/// Veritabanında sayı olarak tutulur: Held = 0, Confirmed = 1, ...
/// </summary>
public enum ReservationStatus
{
    Held,
    Confirmed,
    Expired,
    Cancelled
}
