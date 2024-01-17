using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VisualHomeBackend.Extensions;
using VisualHomeBackend.Models.User;
using VisualHomeBackend.Services;

namespace VisualHomeBackend.Routes
{
    public class LoginRoute
    {
        public static void Map(WebApplication app)
        {            
            app.MapPost("/api/login", async (User user, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
            {

                User? dbUser = null;
                try
                {
                    dbUser = await userDbService.GetUserByName(user.Name);
                }

                catch (AccessDeniedException)
                {
                    return Results.Extensions.InternalError(AccessDeniedException.DefaultMessage);
                }

                catch (Exception ex) 
                {                    
                    return Results.Extensions.InternalError(ex.Message);
                }

                if (dbUser == null || dbUser.Password != user.Password)
                {
                    return Results.BadRequest("Invalid credentials");
                }

                var claims = new List<Claim>
                {
                    // These claims can be retrieved from the token later                    
                    new Claim(UserClaims.NameType, dbUser.Name),
                    new Claim(UserClaims.IdType, dbUser.Id.ToString())
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
