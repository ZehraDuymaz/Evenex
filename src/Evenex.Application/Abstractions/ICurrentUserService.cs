
namespace Evenex.Application.Abstraction;
public interface ICurrentUserService
{
    int Id { get; }
    string Email { get; }
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool IsAuthenticated { get; }
}