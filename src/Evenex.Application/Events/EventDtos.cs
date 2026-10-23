using Evenex.Common.Dtos;

namespace Evenex.Application.Events;

/// <summary>
/// Yeni etkinlik oluşturmak ve güncellemek için gerekli bilgiler.
/// VenueId dışarıya her zaman hash'lenmiş string olarak gider/gelir, gerçek int id asla görünmez.
/// Güncellemede mekân değiştirilemez; o yüzden UpdateEventDto'da VenueId yok.
/// </summary>

public record CreateEventDto (string VenueId, string Title, string Description, DateTime EventDate);
public record UpdateEventDto (string Title, string Description, DateTime EventDate);
public record EventResponseDto (string Id, string VenueId, string Title, string Description, DateTime EventDate, string Status) : BaseDto(Id);
