namespace Evenex.Application.Venues;

public record CreateVenueDto (string Name, string Address);

public record VenueResponseDto(string Id, string Name);
public record UpdateVenueDto (string Name, string Address);