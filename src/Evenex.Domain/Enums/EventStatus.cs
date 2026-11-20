namespace Evenex.Domain.Enums;

/// <summary>
/// Etkinliğin yaşam döngüsü. Sadece bu 3 değer olabilir; "Drfat" gibi yazım hatası derleme hatası verir.
///   Draft     (taslak)  : yeni oluşturuldu, düzenleniyor, henüz kimse göremez/bilet alamaz
///   Published (yayında) : herkese açık, bilet satışı yapılabilir
///   Cancelled (iptal)   : iptal edildi, son durum; geri dönüşü yok
///
/// İzin verilen geçişler (kurallar Event.Publish / Event.Cancel içinde):
///   Draft ──Publish──▶ Published
///   Draft / Published ──Cancel──▶ Cancelled
///
/// Veritabanına sayı (0,1,2) değil İSİM olarak ("Published") yazılır (bkz. AppDbContext).
/// Bu yüzden değerlerin sırasını değiştirmek veriyi bozmaz; ama bir değerin ADINI değiştirmek bozar.
/// </summary>
public enum EventStatus
{
    Draft,
    Published,
    Cancelled
}
