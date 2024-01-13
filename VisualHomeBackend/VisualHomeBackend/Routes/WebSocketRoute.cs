using VisualHomeBackend.Services;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using VisualHomeBackend.Models;

namespace VisualHomeBackend.Routes
{
    public class WebSocketRoute
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/api/ws", async (HttpContext httpContext, UserWsConnectionManagerService userConnectionManagerService, UsersDbService usersDbService) =>
            {
                // This function must be kept active for the entire duration of the websocket connection.
                try
                {
                    if (httpContext.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                        // First message from client must be authentication token
                        var (timeout, (tokenResult, token)) = await AsyncFunctions.ExecuteWithTimeout(
                            UserWsConnection.AwaitTokenMessage(webSocket),
                            TimeSpan.FromSeconds(3));

                        if (timeout || token == null)
                        {
                            Console.WriteLine("Authentication message timed out!");
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                            return;
                        } 
                        else if (tokenResult == TokenMessageResult.Invalid)
                        {
                            Console.WriteLine("Invalid token format in websocket message.");
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                            return;
                        }

                        // Validation of token
                        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                        var claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, JwtParameterFetcher.GetTokenValidationParameters(), out _);
                        string? userId = UserClaims.GetValueOfClaimType(claimsPrincipal.Claims, UserClaims.IdType);                        
                        if (userId == null)
                        {
                            Console.WriteLine($"User ID could not be extracted from claim!");
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                            return;
                        }
                        
                        if (!Guid.TryParse(userId, out Guid userGuid))
                        {
                            Console.WriteLine($"User ID has invalid format!");
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                            return;
                        }                                            

                        var user = await usersDbService.GetUser(userGuid);

                        if (user == null)
                        {                            
                            Console.WriteLine($"User: '{userId}' could not be found in DB!");
                            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        Console.WriteLine($"WebSocketRoute: User with name {user.Name} authenticated.");

                        UserWsConnection userWsConnection = new UserWsConnection(user, webSocket);
                        userConnectionManagerService.AddConnection(userWsConnection);

                        await Task.WhenAll(
                            userWsConnection.ReceiveBytesContinuously(),
                            userWsConnection.SendBytesContinuously());

                    }

                    // Bad request
                    else
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }

                catch (SecurityTokenException ex)
                {
                    Console.WriteLine("Security token exception: " + ex.Message);
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;                    
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }
            });
        }
    }
}
