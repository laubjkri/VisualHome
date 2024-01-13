using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VisualHomeBackend.Models;
using VisualHomeBackend.Services;
using VisualHomeBackend.Extensions;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace VisualHomeBackend.Routes.UserRoutes
{
    public class UpdateCurrentUserRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/user/updatecurrent", async (HttpContext context, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
            {
                // Get details of currently logged in user
                string? userIdString = UserClaims.GetValueOfClaimType(context.User.Claims, UserClaims.IdType);
                if (userIdString == null)
                {
                    return Results.BadRequest("Failed to extract user from request.");
                }

                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return Results.BadRequest("Failed to get user id from request.");
                }

                // Get the new user
                string requestBody;
                using (var reader = new StreamReader(context.Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                User? updatedUser = JsonSerializer.Deserialize<User>(requestBody);

                string? updateUserId = updatedUser?.Id.ToString();

                if (userIdString != updateUserId)
                {
                    return Results.BadRequest($"User cannot update other users. (ID: {updateUserId})");
                }

                // Try to update user
                try
                {
                    await userDbService.UpdateUser(updatedUser);
                    return Results.Ok(updatedUser);
                }

                catch (FailedToUpdateDbException)
                {
                    return Results.Extensions.InternalError("Failed to update user in database.");
                }

                catch (FailedToUpdateCachedUserException)
                {
                    return Results.Extensions.InternalError("Failed to update internal user cache.");
                }

                catch (Exception ex) 
                {
                    return Results.Extensions.InternalError("Unknown internal error when updating user.");
                }
                
            }).RequireAuthorization();
        }
    }
}
