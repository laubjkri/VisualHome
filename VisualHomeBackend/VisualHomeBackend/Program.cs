using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VisualHomeBackend.Routes;
using VisualHomeBackend.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VisualHomeBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);           

            // Configure middleware
            builder.Services.AddAuthentication()      
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = JwtParameterFetcher.GetTokenValidationParameters();
                });

            builder.Services.AddAuthorization();

            // Configure services
            // Services can be added with three lifetime modes:
            // Transient: New instance every time.
            // Scoped: One instance per http request.
            // Singleton: One instance for the application lifetime.
            // AddDbContext is a scoped service.

            builder.Services.AddSingleton<UserWsConnectionManagerService>();
            //builder.Services.AddDbContext<UsersDbContext>(options =>
            //{
            //    // This "AddDbContext" handles concurrency as opposed to AddSingleton
            //    // It will create a new instance on each request.
            //    // This is called the "Unit of Work" pattern and is a "scoped" service.
            //    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!);
            //});

            // DB service
            // Scoped because entity framework requires it
            builder.Services.AddScoped((IServiceProvider provider) =>            
            {
                return new UsersDbService(builder.Configuration.GetConnectionString("PostgreSql")!);
            });

            builder.Services.AddCors();

            var app = builder.Build();

            // Activate the middleware
            // Important to have CORS before other middlewares
            app.UseCors(policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseExceptionHandler();

            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5)
            }); 
            
            RoutesMain.Map(app);

            MqttService mqttService = new MqttService();
            mqttService.StartAsync();

            app.Run();
        }
    }
}