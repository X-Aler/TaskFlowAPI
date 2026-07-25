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
    public async Task<IActionResult> GetLoginAsync([FromBody] LoginDto login)
    {
       var result = await authService.AuthenticateAsync(login);

       if (string.IsNullOrEmpty(result)) return Unauthorized();

       return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto register) => this.HandleStatus(await authService.RegisterAsync(register));
}