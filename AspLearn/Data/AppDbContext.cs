using AspLearn.Models;
using Microsoft.EntityFrameworkCore;

namespace AspLearn.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoTask> Tasks { get; set; }
}