using AspLearn.Controllers;
using AspLearn.Data;
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

            builder.Services.AddSingleton<ITasksService, TaskService>();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
