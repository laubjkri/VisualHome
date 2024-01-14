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
            app.MapPost("/api/user/updatecurrent", async (HttpContext context, UsersDbService userDbService) =>
            {
                // Get details of currently logged in user
                string? userIdString = UserClaims.GetValueOfClaimType(context.User.Claims, UserClaims.IdType);
                if (userIdString == null)                
                    return Results.BadRequest("Failed to extract user from request.");                

                if (!Guid.TryParse(userIdString, out Guid userId))                
                    return Results.BadRequest("Failed to get user id from request.");               

                User? updatedUserRequest = await BodyDeserializer.GetObject<User?>(context.Request);
                if (updatedUserRequest is null)                
                    return Results.BadRequest("A valid user was not provided with the request.");                

                if (updatedUserRequest.Id is null)
                    return Results.BadRequest("A user ID must be supplied with the request");


                string? updatedUserRequestId = updatedUserRequest.Id.ToString();
                if (userIdString != updatedUserRequestId)                
                    return Results.BadRequest($"User cannot update other users. (ID: {updatedUserRequestId})");
                

                // Try to update user
                try
                {
                    // We have to get the current information for the user to support partial updates
                    User? userInDb = await userDbService.GetUserById(userId);
                    if (userInDb is null)
                        return Results.NotFound();

                    userInDb.CopySetProperties(updatedUserRequest);

                    User updated = await userDbService.UpdateUser(userInDb);
                    return Results.Ok(updated);
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
