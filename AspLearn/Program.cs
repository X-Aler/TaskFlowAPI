using AspLearn.Controllers;
using AspLearn.Services;

namespace AspLearn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddSingleton<ITasksService, TaskService>();

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
