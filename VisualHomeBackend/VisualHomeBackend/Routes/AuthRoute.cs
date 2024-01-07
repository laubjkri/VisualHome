using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;

namespace VisualHomeBackend.Routes
{
    public class AuthRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/api/auth", (http) =>
            {
                // Endpoint to check backend user verification
                // Check if user is authorized and then print "Authorized" in DOM                
                foreach (var item in http.Request.Headers)
                {
                    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
                }

                return http.Response.WriteAsync("Auth approved by backend, all good!");
            }).RequireAuthorization();
        }
    }
}
