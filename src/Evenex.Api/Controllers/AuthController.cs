using Microsoft.AspNetCore.Mvc;
using Evenex.Application.Auth;

namespace Evenex.Api.Controllers;

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
    // [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        return Ok(new { message = "You have successfully bypassed the security! The JWT works." });
    }
}