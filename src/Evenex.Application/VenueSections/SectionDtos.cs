using Evenex.Domain.Enums;

namespace Evenex.Application.VenueSections;

public record CreateSectionDto (string VenueId, string Name, SectionType Type);

public record UpdateSectionDto (string Name, SectionType Type);
