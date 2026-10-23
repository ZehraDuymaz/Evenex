namespace Evenex.Domain.Enums;

/// <summary>
/// User: bilet alan kişi, Organizer: etkinlik/mekân oluşturan, Admin: her şeyi yönetebilen.
/// JWT token'ının içine "role" claim'i olarak yazılır.
/// </summary>
public enum UserRole
{
    User,
    Organizer,
    Admin
}
