using System.Text.Json.Serialization;
using Evenex.Common.Hashing;

namespace Evenex.Common.Dtos;

/// <summary>
/// Tüm response DTO'larının miras aldığı ortak record. Id hash'leme işi bir kere burada yapılır,
/// hiçbir DTO'da, MappingProfile'da veya serviste tekrar yazılmaz.
///
/// İki yüzlü bir kimlik kartı gibi çalışır:
///   Id       = gerçek int id. Uygulama içinde kullanılır, JSON'a ASLA yazılmaz ([JsonIgnore]).
///   HashedId = dışarıya giden "k1JppaeX" hali. JSON'da "id" adıyla görünür, frontend hiçbir şey fark etmez.
/// Birini set edersen diğeri kendiliğinden dolar:
///   AutoMapper entity.Id'yi Id'ye kopyalar -> HashedId otomatik hesaplanır (dışarı giden yön)
///   JSON'dan "id" okunur -> HashedId set edilir -> Id otomatik çözülür (içeri gelen yön)
///
/// Neden set değil init? init sadece nesne OLUŞTURULURKEN yazılabilir, sonra kilitlenir.
/// Böylece hem bu otomatik hesaplama çalışıyor hem de DTO oluşturulduktan sonra değiştirilemiyor
/// (record'ların asıl avantajı olan immutability korunuyor). Islak beton gibi: kururken yazılır, sonra sabit.
/// AutoMapper ve JSON okuyucu da nesneyi oluştururken doldurdukları için init'e yazabiliyorlar.
///
/// Audit alanları (CreatedUser vb.) bilerek yok: her response'ta gereksiz veri ve bilgi sızıntısı olurdu.
/// </summary>
public abstract record BaseDto
{
    private int _id;
    private string _hashedId = string.Empty;

    [JsonIgnore]
    public int Id
    {
        get => _id;
        init
        {
            _id = value;
            // 0 = "henüz id yok" (kaydedilmemiş entity); onu hash'lemenin anlamı yok
            if (value > 0)
                _hashedId = HashidsHelper.Encode(value);
        }
    }

    [JsonPropertyName("id")]
    public string HashedId
    {
        get => _hashedId;
        init
        {
            _hashedId = value;
            // Boş geldiyse çözülecek bir şey yok. Geçersizse Decode FormatException fırlatır -> 400.
            if (!string.IsNullOrWhiteSpace(value))
                _id = HashidsHelper.Decode(value);
        }
    }
}
