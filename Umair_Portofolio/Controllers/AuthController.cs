using Microsoft.AspNetCore.Mvc;
using Umair_Portofolio.Dtos;
using Umair_Portofolio.Services;

namespace Umair_Portofolio.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok(new { message = "Registratie gelukt" });
    }
        [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var success = await _service.LoginAsync(dto);
        if (!success) return Unauthorized();

        return Ok(new { message = "Ingelogd" });
    }
}