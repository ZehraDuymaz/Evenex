using Evenex.Application.Abstractions;
using Evenex.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Evenex.Infrastructure.Interceptors;

/// <summary>
/// Audit (denetim) alanlarını TEK bir yerde, otomatik dolduran EF Core interceptor'ı.
/// Interceptor = EF'nin belirli anlarda (burada: SaveChanges'tan hemen önce) kendiliğinden çağırdığı kod.
///
/// Kaydedilmek üzere olan her entity'ye bakar (hepsi BaseEntity'den türediği için hepsinde aynı alanlar var):
///   Yeni eklenen (Added)   -> CreatedUser, CreatedDate, CreatedIP
///   Değiştirilen (Modified) -> ModifiedUser, ModifiedDate, ModifiedIP  (soft delete de bir "değişiklik")
///
/// Eskiden her servis bu alanları elle dolduruyordu (17 satır) ve soft delete hiç doldurmuyordu:
/// silinen kayıtlarda "kim sildi" boş kalıyordu. Artık kimse unutamaz; yeni entity'ler de bedavaya alır.
/// Arşiv kapısındaki damga makinesi gibi: her belge içeri girerken otomatik damgalanır.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditInterceptor(ICurrentUserService currentUser) => _currentUser = currentUser;

    // EF'nin hem senkron hem asenkron kaydetme yolu var; ikisinde de aynı işi yapıyoruz
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        StampAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StampAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;
        var email = _currentUser.Email;
        var ip = _currentUser.IpAddress;

        // ChangeTracker: EF'nin "hangi nesne eklendi / değişti / silindi" listesi.
        // Entries<BaseEntity>(): sadece BaseEntity'den türeyenler (yani tüm entity'lerimiz).
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    // Sadece BOŞSA dolduruyoruz: kayıt olma (register) anında kullanıcı henüz giriş yapmamış,
                    // token yok, Email boş. O durumda AuthService CreatedUser'ı kendisi veriyor;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedUser))
                        entry.Entity.CreatedUser = string.IsNullOrEmpty(email) ? "anonim" : email;
                    if (string.IsNullOrEmpty(entry.Entity.CreatedIP))
                        entry.Entity.CreatedIP = ip;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedDate = now;
                    entry.Entity.ModifiedUser = string.IsNullOrEmpty(email) ? "anonim" : email;
                    entry.Entity.ModifiedIP = ip;
                    break;
            }
        }
    }
}
