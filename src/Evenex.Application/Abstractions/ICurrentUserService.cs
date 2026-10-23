
namespace Evenex.Application.Abstraction;
/// <summary>
/// İsteği yapan (giriş yapmış) kullanıcının bilgilerini JWT token'ından okur.
/// Interface Application'da, implementasyonu Infrastructure'da: Application katmanı HttpContext'i bilmek zorunda kalmaz.
/// </summary>
public interface ICurrentUserService
{
    int Id { get; }
    string Email { get; }
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool IsAuthenticated { get; }
}