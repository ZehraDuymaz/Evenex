using HashidsNet;

namespace Evenex.Common.Hashing;

/// <summary>
/// Hashids'e statik erişim. BaseDto'nun init blokları id'yi kendi kendine hash'leyebilsin diye var:
/// DTO'lar DI ile oluşturulmadığı (AutoMapper veya JSON okuyucu "new"liyor) için constructor'dan
/// IHashids alamazlar, o yüzden buradan alıyorlar.
///
/// Program.cs açılışta DI'daki AYNI Hashids nesnesiyle Configure'u çağırır; böylece DTO'lar,
/// servisler ve JWT hep aynı salt ile aynı hash'leri üretir.
/// </summary>
public static class HashidsHelper
{
    private static IHashids? _hashids;

    // Uygulama açılırken bir kere çağrılır (testlerde de test başında)
    public static void Configure(IHashids hashids) => _hashids = hashids;

    // Configure unutulursa sessizce yanlış çalışmak yerine hemen ve açık bir mesajla patlasın
    private static IHashids Instance =>
        _hashids ?? throw new InvalidOperationException("HashidsHelper.Configure çağrılmadan kullanıldı (Program.cs'e bakın).");

    // 7 -> "k1JppaeX"
    public static string Encode(int id) => Instance.Encode(id);

    // "k1JppaeX" -> 7. Bizim üretmediğimiz (uydurulmuş/bozuk) bir değer gelirse FormatException fırlatır:
    // JSON okunurken fırlatılan FormatException'ı ASP.NET 400 Bad Request'e çevirir, uygulama 500 vermez.
    public static int Decode(string hashedId)
    {
        var decoded = Instance.Decode(hashedId);
        if (decoded.Length == 0)
            throw new FormatException($"Geçersiz id: {hashedId}");
        return decoded[0];
    }
}
