namespace Evenex.Application.Auth;

/// <summary>
/// Yeni hesap oluşturmak, giriş yapmak ve Jwt ile authentication kontrolü yapmak için gerekli bilgiler
/// </summary>
public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Email, string Role);
