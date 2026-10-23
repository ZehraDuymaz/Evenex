using Evenex.Domain.Common;
using Evenex.Domain.Enums;

namespace Evenex.Domain.Entities;

/// <summary>
/// Sisteme kayıtlı kullanıcı. Rolüne göre (User / Organizer / Admin) yapabilecekleri değişir.
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = default!;
    // Şifrenin kendisi ASLA saklanmaz, sadece BCrypt ile hash'lenmiş hali tutulur
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; }
}
