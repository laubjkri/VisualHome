using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;

namespace VisualHomeBackend.Routes
{
    public class LoginRoute
    {
        public static void Map(WebApplication app)
        {            
            app.MapPost("/api/login", async (User user, UsersDbService userDbService, IConfiguration configuration, UserWsConnectionManagerService userConnectionManagerService) => // ASP.net will convert HTML body json to user
            {
                if (await userDbService.CheckUser(user) < 0)
                {
                    return Results.BadRequest("Invalid credentials");
                }

                User dbUser = await userDbService.GetUser(user.Name) ?? throw new Exception("User is null when it should not be");

                var claims = new List<Claim>
                {
                    // These claims can be retrieved from the token later                    
                    new Claim("username", dbUser.Name),
                    new Claim("isAdmin", dbUser.IsAdmin == true ? "true" : "false" ),
                    //new Claim(ClaimTypes.Name, dbUser.Username),
                    //new Claim(ClaimTypes.Role, dbUser.IsAdmin == true ? "admin" : "user"),
                };

                var expiry = DateTime.Now.AddMinutes(60);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: expiry,
                    signingCredentials: JwtParameterFetcher.GetTokenSigningCredentials()
                );               

                return Results.Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });            
            });
        }
    }
}
