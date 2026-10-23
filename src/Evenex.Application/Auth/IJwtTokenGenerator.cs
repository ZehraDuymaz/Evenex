using Evenex.Domain.Entities;

namespace Evenex.Application.Auth;

/// <summary>
/// Kullanıcı için imzalı bir JWT üretir. Implementasyonu Infrastructure/Auth/JwtTokenGenerator.cs'te.
/// </summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}