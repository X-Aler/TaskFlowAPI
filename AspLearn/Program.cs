using AspLearn.Controllers;
using AspLearn.Data;
using AspLearn.Middlewares;
using AspLearn.Repositories;
using AspLearn.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace AspLearn
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Приложение запускается!");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                builder.Services.AddControllers();

                builder.Services.AddScoped<ITasksService, TaskService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<ITasksRepository, TasksRepository>();
                builder.Services.AddScoped<IUsersRepository, UsersRepository>();

                builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Token:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Token:Audience"],

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Token:SecretKey"]!))
                    }
                );

                builder.Services.AddAuthorization();

                var app = builder.Build();

                app.UseMiddleware<GlobalExceptionMiddleware>();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                Log.Information("Приложение успешно запустилось!");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Приложение не удалось запустить!");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
