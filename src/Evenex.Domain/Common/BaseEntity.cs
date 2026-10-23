namespace Evenex.Domain.Common;

/// <summary>
/// Tüm entity'lerin miras aldığı ortak temel sınıf. Oluşturulma/güncellenme
/// bilgisi ve soft delete alanlarını içerir.
/// abstract: tek başına "new BaseEntity()" yapılamaz, sadece miras alınabilir.
/// </summary>
public abstract class BaseEntity
{
    // Veritabanındaki gerçek id (otomatik artan int). Dışarıya asla bu haliyle verilmez;
    // response'larda Hashids ile "wk1JaEWX" gibi bir string'e çevrilir (bkz. MappingProfile).
    public int Id { get; set; }

    // Audit (denetim) alanları: kaydı kim, ne zaman, hangi IP'den oluşturdu / değiştirdi.
    // "= default!" derleyiciye "bu null olmayacak, söz veriyorum" demek; değerini servis set ediyor.
    public string CreatedUser { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public string CreatedIP { get; set; } = default!;

    // "?" = nullable. Kayıt hiç güncellenmediyse bu alanlar boş (NULL) kalır.
    public string? ModifiedUser { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedIP { get; set; }

    // Soft delete: kayıt veritabanından fiziksel olarak silinmez, sadece true yapılır.
    // Böylece geçmiş siparişler/biletler bozulmaz ve gerekirse geri alınabilir.
    public bool IsDeleted { get; set; }
}
