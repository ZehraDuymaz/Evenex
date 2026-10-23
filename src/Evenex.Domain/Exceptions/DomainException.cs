namespace Evenex.Domain.Exceptions;

/// <summary>
/// İş kuralı ihlallerinin (kapasite dolu, koltuk dolu...) ortak atası.
/// GlobalExceptionHandler tüm DomainException'ları 409 Conflict olarak döner.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
