using AspLearn.Models;
using AspLearn.Models.DTOs;

namespace AspLearn.Services;

public interface IAuthService
{
    string? Authenticate(LoginDto login);
}