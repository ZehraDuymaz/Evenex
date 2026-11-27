namespace Evenex.Domain.Exceptions;

/// <summary>
/// Aynı bölüm aynı etkinliğe ikinci kez eklenmeye çalışıldığında fırlatılır.
/// DomainException olduğu için 409 Conflict döner. (Veritabanındaki unique index de aynı şeyi engelliyor;
/// bu kontrol, kullanıcıya anlaşılır bir mesaj verebilmek için ondan önce yapılıyor.)
/// </summary>
public class DuplicateEventSectionException : DomainException
{
    public DuplicateEventSectionException()
        : base("Bu bölüm bu etkinliğe zaten eklenmiş.") { }
}
