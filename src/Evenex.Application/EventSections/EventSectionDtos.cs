using Evenex.Common.Dtos;

namespace Evenex.Application.EventSections;


/// <summary>
/// Yeni etkinlik bölümü oluşturmak ve güncellemek için gerekli bilgiler
/// EventSectionResponseDto, EventSection bilgisi alınarak BasePrice, Capacity ve Discount ile bağlanması içindir.
/// </summary>

public record CreateEventSectionDto (string EventId, string SectionId, decimal BasePrice, int Capacity);
public record UpdateEventSectionDto (decimal BasePrice, int Capacity);

// Kendi id'si BaseDto'da, SectionId ve EventId (foreign key'ler) MappingProfile'da hash'lenir
public record EventSectionResponseDto : BaseDto
{
    public string SectionId { get; init; } = default!;
    public string EventId { get; init; } = default!;
    public decimal BasePrice { get; init; }
    public int Capacity { get; init; }
    public int RemainingCapacity { get; init; }
}
