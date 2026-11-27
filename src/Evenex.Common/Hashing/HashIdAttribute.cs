using System.Text.Json.Serialization;

namespace Evenex.Common.Hashing;

/// <summary>
/// "Bu int bir id'dir; dış dünyada hash'li görünsün" etiketi. Tek etiket, iki yerde çalışır:
///
///   1. DTO property'sinde (JSON gövdesi):
///        public record CreateEventDto([property: HashId] int VenueId, ...)
///      JsonConverterAttribute'tan türediği için JSON okuyucu/yazıcı HashIdJsonConverter'ı kullanır.
///
///   2. Controller parametresinde (URL):
///        public IActionResult GetById([HashId] int id)
///      Evenex.Api'deki HashIdModelBinderProvider bu etiketi görür ve URL'deki hash'i int'e çevirir.
///
/// AttributeUsage: hem property'ye hem metot parametresine konabilsin diye
/// (JsonConverterAttribute normalde parametreye konamaz, burada izin veriyoruz).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class HashIdAttribute : JsonConverterAttribute
{
    public HashIdAttribute() : base(typeof(HashIdJsonConverter)) { }
}
