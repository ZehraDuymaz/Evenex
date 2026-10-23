namespace Evenex.Domain.Exceptions;

/// <summary>Ayakta bölümde yer kalmadığında fırlatılır.</summary>
public class CapacityExceededException : DomainException
{
    public CapacityExceededException(int eventSectionId)
        : base($"No remaining capacity for event section {eventSectionId}.") { }
}
