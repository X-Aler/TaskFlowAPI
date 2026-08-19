using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public interface IAuthService
{
    Task<string> LoginAsync(LoginDto login);
    Task RegisterAsync(RegisterDto register);
}