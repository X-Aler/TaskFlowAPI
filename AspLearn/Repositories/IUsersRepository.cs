using AspLearn.Models;

namespace AspLearn.Repositories;

public interface IUsersRepository
{
    Task<User?> GetUserByLoginAsync(string login);
    Task AddUserAsync(User user);
}