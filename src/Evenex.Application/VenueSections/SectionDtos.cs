using Evenex.Common.Dtos;
using Evenex.Domain.Enums;

namespace Evenex.Application.VenueSections;

/// <summary>
/// Mekân bölümü (Section) oluşturma/güncelleme ve dışarıya gösterme bilgileri.
/// Type: "Seated" (oturmalı) veya "Standing" (ayakta). JSON'da isim olarak gider/gelir (eski usul 0/1 de kabul edilir).
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
