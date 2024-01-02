using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;

namespace VisualHomeBackend.Routes
{
    public class AddUserRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/adduser", async (HttpContext context, User user, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
            {
                // Check that user has the ability to create users
                var authUser = context.User;
                if (authUser.Identity is null || !authUser.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
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
