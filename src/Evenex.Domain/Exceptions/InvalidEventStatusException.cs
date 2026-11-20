using Evenex.Domain.Enums;

namespace Evenex.Domain.Exceptions;

/// <summary>
/// İzin verilmeyen bir durum geçişi denendiğinde fırlatılır (örn. iptal edilmiş etkinliği yayınlamak).
/// DomainException'dan türediği için GlobalExceptionHandler bunu 409 Conflict olarak döner:
/// istek düzgün ama kaydın şu anki durumuyla çelişiyor.
/// </summary>
public class InvalidEventStatusException : DomainException
{
    public InvalidEventStatusException(EventStatus current, string action)
        : base($"Etkinlik '{current}' durumundayken {action} yapılamaz.") { }
}
