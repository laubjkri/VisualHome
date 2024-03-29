﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VisualHomeBackend.Services;
using VisualHomeBackend.Extensions;
using VisualHomeBackend.Models.User;

namespace VisualHomeBackend.Routes.UserRoutes
{
    public class CreateUserRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/user/create", async (HttpContext context, User newUser, UsersDbService userDbService) => // ASP.net will convert HTML body json to user
            {                

                // User should be admin to create users
                string? userId = UserClaims.GetValueOfClaimType(context.User.Claims, UserClaims.IdType);
                if (!Guid.TryParse(userId, out Guid userIdGuid))
                {
                    return Results.Extensions.InternalError("Failed to extract user id from request.");
                }

                User? loggedInUser = await userDbService.GetUserById(userIdGuid);

                if (loggedInUser == null)
                {
                    return Results.Extensions.InternalError("Current user was not found in database.");
                }

                if (loggedInUser.IsAdmin is null || !(bool)loggedInUser.IsAdmin)
                {                    
                    return Results.Extensions.Unauthorized("Currently logged in user is not allowed to create users.");
                }

                try
                {
                    await userDbService.CreateUser(newUser);
                    return Results.Ok(newUser);
                }

                catch (DbConcurrencyException ex)
                {
                    return Results.Extensions.Locked("Concurrency error in database.");
                }

                catch (DbUpdateException ex)
                {
                    return Results.NotFound("Database update error: " + ex.Message);
                }

                catch (Exception ex)
                {
                    return Results.Extensions.InternalError("Internal server error: " + ex.Message);
                }                                

            }).RequireAuthorization();
        }
    }
}
