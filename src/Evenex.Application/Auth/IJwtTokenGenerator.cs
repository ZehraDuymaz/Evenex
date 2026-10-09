using Evenex.Domain.Entities;

namespace Evenex.Application.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}