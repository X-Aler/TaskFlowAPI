using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public interface IAuthService
{
    Task<string> AuthenticateAsync(LoginDto login);
    Task RegisterAsync(RegisterDto register);
}