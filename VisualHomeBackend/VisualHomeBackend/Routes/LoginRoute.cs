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
            app.MapPost("/api/login", async (UserModel user, UsersDbService userService, IConfiguration configuration, UserWsConnectionManagerService userConnectionManagerService) => // ASP.net will convert HTML body json to user
            {
                if (await userService.CheckUser(user) < 0)
                {
                    return Results.BadRequest("Invalid credentials");
                }

                UserModel dbUser = await userService.GetUser(user.Username) ?? throw new Exception("User is null when it should not be");

                var claims = new List<Claim>
                {
                    // These claims can be retrieved from the token later                    
                    new Claim("username", dbUser.Username),
                    new Claim("canEditUsers", dbUser.CanEditUsers == true ? "true" : "false" )
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
