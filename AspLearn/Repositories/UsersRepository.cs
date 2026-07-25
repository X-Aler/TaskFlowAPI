using AspLearn.Data;
using AspLearn.Models;
using Microsoft.EntityFrameworkCore;

namespace AspLearn.Repositories;

public class UsersRepository(AppDbContext dbContext) : IUsersRepository
{
    public async Task<User?> GetUserByLoginAsync(string login) => await dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

    public async Task AddUserAsync(User user)
    {
        await dbContext.Users.AddAsync(user);

        await dbContext.SaveChangesAsync();
    }
}