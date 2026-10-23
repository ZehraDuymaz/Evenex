using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Evenex.Application.Auth;

namespace Evenex.Api.Controllers;

/// <summary>
/// Kayıt ve giriş endpoint'leri: localhost:5177/api/auth/register ve /api/auth/login
/// İkisi de herkese açık (token almak için zaten token olamaz).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authservice;
    public AuthController(AuthService authService) => _authservice = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            return Ok(await _authservice.RegisterAsync(request));
        } 
        // Email zaten kayıtlıysa 409 Conflict
        catch (InvalidOperationException exErr)
        {
            return Conflict(exErr.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            return Ok(await _authservice.LoginAsync(request));
        } 
        catch (UnauthorizedAccessException exErr)
        {
            return Unauthorized(exErr.Message);
        }
    }
    
    // JWT'nin çalıştığını hızlıca test etmek için. [Authorize] olduğu için token olmadan 401 döner.
    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        return Ok(new { message = "JWT çalışıyor!" });
    }
}