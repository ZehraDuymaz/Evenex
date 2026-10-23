using Evenex.Domain.Common;

namespace Evenex.Domain.Entities;

/// <summary>
/// Kullanıcıya verilen bilet. Girişte QR kod okutularak doğrulanır.
/// </summary>
public class Ticket : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public string QrCode { get; set; } = default!;
    // Valid (geçerli) -> girişte kullanılınca "Used" gibi bir değere çekilecek
    public string Status { get; set; } = "Valid";
}
