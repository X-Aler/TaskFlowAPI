using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspLearn.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> GetLoginAsync([FromBody] LoginDto login) => Ok(await authService.AuthenticateAsync(login));

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto register)
    {
        await authService.RegisterAsync(register);

        return Ok();
    }
}