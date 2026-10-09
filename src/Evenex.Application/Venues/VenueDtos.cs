namespace Evenex.Application.Venues;

public record CreateVenueDto (string Name, string Address);

public record UpdateVenueDto (string Name, string Address);