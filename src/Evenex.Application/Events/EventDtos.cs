namespace Evenex.Application.Events;

/// <summary>
/// Yeni etkinlik oluşturmak ve güncellemek için gerekli bilgiler
/// /// </summary>

public record CreateEventDto (int VenueId, string Title, string Description, DateTime EventDate);
public record UpdateEventDto (int VenueId, string Title, string Description, DateTime EventDate);
