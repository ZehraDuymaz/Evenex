using Evenex.Domain.Common;

namespace Evenex.Domain.Entities;

/// <summary>
/// Sipariş: geçici bir rezervasyonun (Reservation) ödemesi yapılıp kesinleşmiş hali.
/// Her siparişin bir rezervasyonu ve (oluşturulduysa) bir bileti vardır.
/// </summary>
public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = default!;

    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Confirmed";

    // "?" çünkü bilet sipariş kaydedildikten sonra oluşturuluyor, ilk anda null olabilir
    public Ticket? Ticket { get; set; }
}
