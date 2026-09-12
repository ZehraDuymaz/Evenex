namespace Evenex.Application.Events;

/// <summary>
/// Yeni etkinlik oluşturmak ve güncellemek için gerekli bilgiler
/// EventResponseDto, Event bilgisi alınarak EventSection ve Discount ile bağlanması içindir. 
/// /// </summary>

public record CreateEventDto (string VenueId, string Title, string Description, DateTime EventDate);
public record UpdateEventDto (int VenueId, string Title, string Description, DateTime EventDate); // check id later!
public record EventResponseDto (string Id, string Title, string Description, DateTime EventDate);
