using Evenex.Common.Dtos;
using Evenex.Domain.Enums;

namespace Evenex.Application.VenueSections;

/// <summary>
/// Mekân bölümü (Section) oluşturma/güncelleme ve dışarıya gösterme bilgileri.
/// Type: 0 = Seated (oturmalı), 1 = Standing (ayakta).
/// </summary>
public record CreateSectionDto (string VenueId, string Name, SectionType Type);

public record UpdateSectionDto (string Name, SectionType Type);

// Kendi id'si BaseDto'da, VenueId (foreign key) MappingProfile'da hash'lenir
public record SectionResponseDto : BaseDto
{
    public string VenueId { get; init; } = default!;
    public string Name { get; init; } = default!;
    public SectionType Type { get; init; }
}
