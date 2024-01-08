using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;
using VisualHomeBackend.Extensions;

namespace VisualHomeBackend.Routes.UserRoutes
{
    public class CreateUserRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/createuser", async (HttpContext context, User newUser, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
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
                User loggedInUser = await userDbService.GetUser(username) ?? throw new Exception("System error: Logged in user is not found in database.");

                if (!loggedInUser.IsAdmin)
                {                    
                    return Results.Extensions.UnauthorizedResponse("Currently logged in user is not allowed to create users.");
                }

                try
                {
                    await userDbService.CreateUser(newUser);
                    return Results.Ok(newUser);
                }

                catch (DbConcurrencyException ex)
                {
                    return Results.Extensions.LockedResponse("Concurrency error in database.");
                }

                catch (DbUpdateException ex)
                {
                    return Results.NotFound("Database update error: " + ex.Message);
                }

                catch (Exception ex)
                {
                    return Results.Extensions.InternalErrorResponse("Internal server error: " + ex.Message);
                }                                

            });
        }
    }
}
