using HashidsNet;
using Evenex.Application.Abstraction;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Evenex.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHashids _hashids;
    public CurrentUserService (IHttpContextAccessor httpContextAccessor, IHashids hashids)
    {
        _httpContextAccessor = httpContextAccessor;
        _hashids = hashids;
    }
    public int Id
    {
        get
        {
            var idClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);  
            if (string.IsNullOrEmpty(idClaim)) return 0;
            var decoded = _hashids.Decode(idClaim);
            if (decoded.Length > 0) return decoded[0];
            return 0;
        }
    }    
    public string Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public IReadOnlyCollection<string> Roles =>
        _httpContextAccessor.HttpContext?.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? [];

    public IReadOnlyCollection<string> Permissions =>
        _httpContextAccessor.HttpContext?.User
            .FindAll("permission")
            .Select(c => c.Value)
            .ToList() ?? [];

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}