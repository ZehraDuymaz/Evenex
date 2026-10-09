
namespace Evenex.Application.EventSections;


/// <summary>
/// Yeni etkinlik bölümü oluşturmak ve güncellemek için gerekli bilgiler
/// EventResponseDto, Event bilgisi alınarak EventSection ve Discount ile bağlanması içindir. 
/// /// </summary>

public record CreateEventSectionDto (string EventId, string Title, string Description, DateTime EventDate);
public record UpdateEventSectionDto (int EventId, string Title, string Description, DateTime EventDate); 
public record EventResponseDto (string Id, string Title, string Description, DateTime EventDate);
