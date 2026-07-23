using AspLearn.Controllers;
using AspLearn.Data;
using AspLearn.Repositories;
using AspLearn.Services;
using Microsoft.EntityFrameworkCore;

namespace AspLearn
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddScoped<ITasksService, TaskService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITasksRepository, TasksRepository>();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
