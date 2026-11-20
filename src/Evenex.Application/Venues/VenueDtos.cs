using Evenex.Common.Dtos;
namespace Evenex.Application.Venues;

/// <summary>
/// Yeni mekan oluşturma ve güncelleme işlemleri.
/// İstek (request) DTO'ları tek satırlık positional record: sadece gelen formu taşırlar, kendi id'leri yok
/// (güncellenecek kaydın id'si URL'den gelir).
/// VenueResponseDto ise BaseDto'dan miras alır; id'si orada otomatik hash'lenir.
/// </summary>

public record CreateVenueDto (string Name, string Address);
public record UpdateVenueDto (string Name, string Address);

// Positional değil, init property'li record: BaseDto'nun Id/HashedId init mantığı ancak böyle çalışıyor
// (positional record değerleri constructor'dan alır; BaseDto'nun ise parametresiz oluşturulup doldurulması gerekir).
public record VenueResponseDto : BaseDto
{
    public string Name { get; init; } = default!;
    public string Address { get; init; } = default!;
    public int Capacity { get; init; }
}
