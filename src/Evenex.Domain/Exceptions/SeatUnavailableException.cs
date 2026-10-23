namespace Evenex.Domain.Exceptions;

/// <summary>Seçilen koltuk başkası tarafından tutulmuş ya da satılmışsa fırlatılır.</summary>
public class SeatUnavailableException : DomainException
{
    public SeatUnavailableException(int seatId)
        : base($"Seat {seatId} is not available to hold.") { }
}
