
using Evenex.Common.Dtos;

namespace Evenex.Application.EventSections;


/// <summary>
/// Yeni etkinlik bölümü oluşturmak ve güncellemek için gerekli bilgiler
/// EventSectionResponseDto, EventSection bilgisi alınarak BasePrice, Capacity ve Discount ile bağlanması içindir. 
/// /// </summary>

public record CreateEventSectionDto (string EventId, string SectionId, decimal BasePrice, int Capacity);
public record UpdateEventSectionDto (int BasePrice, int Capacity); 
public record EventSectionResponseDto (string Id, string SectionId, string EventId, decimal BasePrice, int Capacity, int RemainingCapacity) : BaseDto(Id);
