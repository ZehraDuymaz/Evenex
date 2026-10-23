using Evenex.Common.Dtos;
using Evenex.Domain.Enums;

namespace Evenex.Application.VenueSections;

/// <summary>
/// Mekân bölümü (Section) oluşturma/güncelleme ve dışarıya gösterme bilgileri.
/// Type: 0 = Seated (oturmalı), 1 = Standing (ayakta).
/// </summary>
public record CreateSectionDto (string VenueId, string Name, SectionType Type);

public record UpdateSectionDto (string Name, SectionType Type);

public record SectionResponseDto (string Id, string VenueId, string Name, SectionType Type) : BaseDto(Id);
