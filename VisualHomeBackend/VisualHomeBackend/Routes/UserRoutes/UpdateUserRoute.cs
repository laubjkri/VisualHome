using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;
using VisualHomeBackend.Extensions;

namespace VisualHomeBackend.Routes.UserRoutes
{
    public class UpdateUserRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/updateuser", async (HttpContext context, User updatedUser, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
            {
                // Manual check that user is authenticated. Could also be done with .RequireAuthorization();
                // Requires valid JWT.
                var authUser = context.User;
                if (authUser.Identity is null || !authUser.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }

                // Get details of currently logged in user
                string username = UserClaims.GetNameValue(context.User.Claims);
                User loggedInUser = await userDbService.GetUser(username) ?? throw new Exception("System error: Logged in user is not found in database.");

                if (loggedInUser.IsAdmin)
                {
                    if (loggedInUser.Name != updatedUser.Name)
                    {
                        User? userToBeUpdated = await userDbService.GetUser(updatedUser.Name);                         
                        if (userToBeUpdated == null)
                        {                            
                            return Results.NotFound($"User '{updatedUser.Name}' not found.");
                        }
                        
                        


                    }

                }
                else // not admin
                {
                    if (loggedInUser.Name != updatedUser.Name)
                    {
                        return Results.Extensions.UnauthorizedWithMessage("It is not possible to change username.");
                    }


                }





                

                // Update the user.
                // Currently the username cannot be updated since it is used as a primary key. This could be changed by adding an ID.





                return Results.Ok(updatedUser);

            });
        }
    }
}
