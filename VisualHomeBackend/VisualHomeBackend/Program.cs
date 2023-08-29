using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VisualHomeBackend.Routes;
using VisualHomeBackend.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

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
            builder.Services.AddSingleton<UserWsConnectionManagerService>();
            builder.Services.AddDbContext<DatabaseContextService>(options =>
            {
                options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!);
            });
            builder.Services.AddSingleton<UsersDbService>();
            builder.Services.AddCors();




            var app = builder.Build();

            // Activate the middleware
            // Impotant to have CORS before other middlewares
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


            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5)
            }); 
            
            RoutesMain.Map(app);

            app.Run();
        }
    }
}