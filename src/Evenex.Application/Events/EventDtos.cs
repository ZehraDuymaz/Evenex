using Evenex.Common.Dtos;

namespace Evenex.Application.Events;

/// <summary>
/// Yeni etkinlik oluşturmak ve güncellemek için gerekli bilgiler.
/// VenueId dışarıya her zaman hash'lenmiş string olarak gider/gelir, gerçek int id asla görünmez.
/// Güncellemede mekân değiştirilemez; o yüzden UpdateEventDto'da VenueId yok.
/// </summary>

public record CreateEventDto (string VenueId, string Title, string Description, DateTime EventDate);
public record UpdateEventDto (string Title, string Description, DateTime EventDate);

// Etkinliğin kendi id'si BaseDto'da otomatik hash'lenir.
// VenueId bir foreign key; BaseDto sadece DTO'nun KENDİ id'sini kapsadığı için onu MappingProfile hash'liyor.
public record EventResponseDto : BaseDto
{
    public string VenueId { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public DateTime EventDate { get; init; }
    public string Status { get; init; } = default!;
}
