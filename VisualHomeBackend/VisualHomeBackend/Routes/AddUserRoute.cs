using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;
using VisualHomeBackend.Extensions;

namespace VisualHomeBackend.Routes
{
    public class AddUserRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/adduser", async (HttpContext context, User newUser, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
            {
                // Manual check that user is authenticated. Could also be done with .RequireAuthorization();
                // Requires valid JWT.
                var authUser = context.User;
                if (authUser.Identity is null || !authUser.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }

                // User should be admin to create users
                string username = UserClaims.GetNameValue(context.User.Claims);
                User loggedInUser = await userDbService.GetUser(username) ?? throw new Exception("User is null when it should not be");                
                if (loggedInUser == null) 
                {
                    return Results.Extensions.UnauthorizedWithMessage("Currently logged in user is not known in database");                    
                }
                
                if(!loggedInUser.IsAdmin)
                {
                    return Results.Extensions.UnauthorizedWithMessage("Currently logged in user is not allowed to create users.");
                }

                await userDbService.AddUser(newUser);

                return Results.Ok(newUser);
                
            });
        }
    }
}
