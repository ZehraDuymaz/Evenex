using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Evenex.Infrastructure.Converters;

/// <summary>
/// Veritabanındaki tüm DateTime'ların UTC olarak okunmasını ve yazılmasını garanti eder.
///
/// Sorun: SQL Server'ın datetime2 kolonu saat dilimi bilgisi TUTMAZ, sadece "2026-10-30 08:18" saklar.
/// EF Core okurken bu yüzden DateTime.Kind = Unspecified ("hangi saat dilimi, bilinmiyor") verir.
/// JSON'a da sonunda "Z" olmadan yazılır ("2026-10-30T08:18:00"); tarayıcı bunu YEREL saat sanar
/// ve Türkiye'de saat 3 saat geri görünür.
///
/// Çözüm: biz zaten her şeyi UTC kaydediyoruz (DateTime.UtcNow). Okurken "bu UTC'dir" etiketini
/// (Kind = Utc) geri yapıştırıyoruz; JSON da "...Z" ile çıkıyor ve tarayıcı doğru çeviriyor.
/// Kolonun tipi değişmediği için migration gerekmez.
/// </summary>
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            // Veritabanına YAZARKEN: yerel saat geldiyse UTC'ye çevir; etiketsiz (Unspecified) geldiyse
            // zaten UTC kabul et (bizim kuralımız: uygulama içinde her zaman UTC).
            v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            // Veritabanından OKURKEN: değeri değiştirmeden sadece "bu UTC" etiketini yapıştır
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}
