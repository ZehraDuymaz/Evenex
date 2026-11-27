using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evenex.Common.Hashing;

/// <summary>
/// JSON'daki hash'li id'ler ile C#'taki gerçek int id'ler arasında çeviri yapan converter.
/// JSON okuyucu/yazıcı (System.Text.Json) [HashId] etiketli bir int alanla karşılaşınca bunu çağırır.
///
///   Okurken (istek gövdesi): "k1JppaeX" -> 7
///   Yazarken (cevap):        7 -> "k1JppaeX"
///
/// Böylece servisler ve AutoMapper hep gerçek int ile çalışır; hash sadece JSON'un içinde (dış dünyada) yaşar.
/// </summary>
public class HashIdJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Sadece hash string'i kabul ediyoruz. Biri gerçek sayıyı (7) gönderirse reddediyoruz:
        // aksi halde tahmin edilebilir id'lerle istek atılabilirdi, Hashids'in amacı bozulurdu.
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Id, hash'lenmiş metin olarak gönderilmeli.");

        var hashed = reader.GetString();
        // JsonException'ı ASP.NET otomatik olarak 400 Bad Request'e çevirir
        if (string.IsNullOrWhiteSpace(hashed) || !HashidsHelper.TryDecode(hashed, out var id))
            throw new JsonException($"Geçersiz id: {hashed}");

        return id;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        => writer.WriteStringValue(HashidsHelper.Encode(value));
}
