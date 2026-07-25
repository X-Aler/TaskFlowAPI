using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspLearn.Services;

public class AuthService(IConfiguration config, IUsersRepository repository) : IAuthService
{
    public async Task<string?> AuthenticateAsync(LoginDto login)
    {
        var user = await repository.GetUserByLoginAsync(login.Login);

        if (user is null) return null;
        if (!string.Equals(user.Password, login.Password)) return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.Login),
            new Claim("Id", user.Id.ToString())
        };

        var secretKey = config["Token:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Token:Issuer"],
            audience: config["Token:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;

    }

    public async Task<ServiceResult> RegisterAsync(RegisterDto register)
    {
        var isLoginTaken = await repository.GetUserByLoginAsync(register.Login) is not null;

        if (isLoginTaken) return ServiceResult.BadRequest;

        var user = new User
        {
            Login = register.Login,
            Password = register.Password
        };

        await repository.AddUserAsync(user);

        return ServiceResult.Ok;
    }
}