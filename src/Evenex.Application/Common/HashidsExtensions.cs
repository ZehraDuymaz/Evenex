using HashidsNet;

namespace Evenex.Application.Common;

/// <summary>
/// Dışarıdan (frontend'den) gelen hash'lenmiş id'leri ("wk1JaEWX" gibi) veritabanındaki
/// gerçek int id'ye çeviren yardımcı metot. Her serviste aynı 3 satırı tekrar yazmamak için var.
/// </summary>
public static class HashidsExtensions
{
    public static int DecodeId(this IHashids hashids, string hashedId, string entityName)
    {
        var decoded = hashids.Decode(hashedId);

        // Decode boş dönerse id bizim ürettiğimiz bir id değildir (uydurulmuş ya da bozulmuş).
        // Kullanıcı açısından bu "kayıt yok" ile aynı şey, o yüzden 404'e dönüşen KeyNotFoundException atıyoruz.
        if (decoded.Length == 0)
            throw new KeyNotFoundException($"{entityName} bulunamadı: {hashedId}");

        return decoded[0];
    }
}
