using Evenex.Domain.Common;

namespace Evenex.Domain.Entities;

/// <summary>
/// Etkinlik. !!!! COMMENT
/// </summary>

public class Event : BaseEntity
{
    public int VenueId { get; set; }
    public Venue Venue { get; set; } = default!;

    public string Title { get; set; } = default!;
    public required string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Status { get; set; } = "Draft";

    public ICollection<EventSection> EventSections { get; set; } = new List<EventSection>();
}
