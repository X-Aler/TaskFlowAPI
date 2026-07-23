using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace AspLearn.Services;

public class AuthService(IConfiguration config) : IAuthService
{
    public string? Authenticate(LoginDto login)
    {
        if (login is not { Login: "admin", Password: "password" }) return null;

        var claim = new Claim(ClaimTypes.Name, login.Login);

        var secretKey = config["Token:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Token:Issuer"],
            audience: config["Token:Audience"],
            claims: new List<Claim>{claim},
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;

    }
}