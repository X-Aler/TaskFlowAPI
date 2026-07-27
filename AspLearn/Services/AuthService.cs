using AspLearn.Exceptions;
using AspLearn.Models;
using AspLearn.Models.DTOs;
using AspLearn.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspLearn.Services;

public class AuthService(IConfiguration config, IUsersRepository repository, ILogger<AuthService> logger) : IAuthService
{
    public async Task<string> AuthenticateAsync(LoginDto login)
    {
        logger.LogInformation("Пользователь {Login} начал аутентификацию.", login.Login);

        var user = await repository.GetUserByLoginAsync(login.Login);

        if (user is null)
        {
            logger.LogWarning("Пользователь {Login} не найден.", login.Login);
            throw new UnauthorizedException($"Пользователь {login.Login} не найден.");
        }

        logger.LogDebug("Пользователь {Login} найден.", login.Login);

        var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
        if (!isPasswordCorrect)
        {
            logger.LogWarning("Пользователь {Login} ввел неверный пароль.", login.Login);
            throw new UnauthorizedException($"Пользователь {login.Login} ввел неверный пароль.");
        }

        logger.LogDebug("Пользователь {Login} ввел верный пароль.", login.Login);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.Login),
            new Claim("Id", user.Id.ToString())
        };

        var secretKey = config["Token:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Token:Issuer"],
            audience: config["Token:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        logger.LogInformation("Аутентификацию пользователя {Login} прошла успешно.", login.Login);

        return tokenValue;

    }

    public async Task RegisterAsync(RegisterDto register)
    {
        logger.LogInformation("Пользователь {Login} начал регистрацию.", register.Login);

        var isLoginTaken = await repository.GetUserByLoginAsync(register.Login) is not null;

        if (isLoginTaken)
        {
            logger.LogWarning("Пользователь c логином {Login} уже существует.", register.Login);
            throw new BadRequestException($"Пользователь c логином {register.Login} уже существует.");
        }

        logger.LogDebug("Логин {Login} свободен.", register.Login);

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

        var user = new User
        {
            Login = register.Login,
            Password = hashPassword
        };

        await repository.AddUserAsync(user);

        logger.LogInformation("Пользователь {Login} успешно прошел регистрацию.", register.Login);
    }
}