using Evenex.Common.Dtos;
using Evenex.Common.Hashing;

namespace Evenex.Application.EventSections;


/// <summary>
/// Yeni etkinlik bölümü oluşturmak ve güncellemek için gerekli bilgiler
/// EventSectionResponseDto, EventSection bilgisi alınarak BasePrice, Capacity ve Discount ile bağlanması içindir.
/// </summary>

public record CreateEventSectionDto ([property: HashId] int EventId, [property: HashId] int SectionId, decimal BasePrice, int Capacity);
public record UpdateEventSectionDto (decimal BasePrice, int Capacity);

// Kendi id'si BaseDto'da, SectionId ve EventId (foreign key'ler) [HashId] ile JSON'da hash'lenir
public record EventSectionResponseDto : BaseDto
{
    [HashId] public int SectionId { get; init; }
    [HashId] public int EventId { get; init; }
    public decimal BasePrice { get; init; }
    public int Capacity { get; init; }
    public int RemainingCapacity { get; init; }
}
