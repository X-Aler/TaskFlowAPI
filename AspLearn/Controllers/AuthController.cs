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
    public IActionResult GetLogin([FromBody] LoginDto login)
    {
       var result = authService.Authenticate(login);

       if (string.IsNullOrEmpty(result)) return Unauthorized();

       return Ok(result);
    }
}