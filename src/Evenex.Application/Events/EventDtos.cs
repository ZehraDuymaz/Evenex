using Evenex.Common.Dtos;
using Evenex.Common.Hashing;
using Evenex.Domain.Enums;

namespace Evenex.Application.Events;

/// <summary>
/// Yeni etkinlik oluşturmak ve güncellemek için gerekli bilgiler.
/// VenueId C#'ta gerçek int; [HashId] sayesinde JSON'da her zaman hash'li metin olarak gider/gelir
/// ("k1JppaeX"), gerçek sayı dışarıda asla görünmez (bkz. HashIdJsonConverter).
/// Güncellemede mekân değiştirilemez; o yüzden UpdateEventDto'da VenueId yok.
/// </summary>

public record CreateEventDto ([property: HashId] int VenueId, string Title, string Description, DateTime EventDate);
public record UpdateEventDto (string Title, string Description, DateTime EventDate);

// Etkinliğin kendi id'si BaseDto'da otomatik hash'lenir.
// VenueId bir foreign key: [HashId] ile JSON'a yazılırken hash'leniyor (BaseDto sadece KENDİ id'sini kapsıyor).
public record EventResponseDto : BaseDto
{
    [HashId] public int VenueId { get; init; }
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public DateTime EventDate { get; init; }
    // Enum; JSON'da sayı değil isim olarak görünür ("Published"), bkz. Program.cs JsonStringEnumConverter
    public EventStatus Status { get; init; }
}
