using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using VisualHomeBackend.Models;

namespace VisualHomeBackend.Services
{
    /// <summary>
    /// This class handles each web socket connection from any user
    /// </summary>
    public class UserWsConnection
    {
        public User User { get; private set; }
        public string ConnectionId { get; }
        public WebSocket WebSocketInstance { get; }
        public bool ConnectionIsActive 
        { 
            get
            {
                // The websocket state will only get updated if a receive is called.
                return WebSocketInstance.State == WebSocketState.Open;
            } 
        }

        public UserWsConnection(User user, WebSocket webSocket)
        {
            ConnectionId = Guid.NewGuid().ToString();            
            WebSocketInstance = webSocket;                     
            User = user;
        }

        public async Task ReceiveBytesContinuously()
        {
            string errorMessage = "";
            TimeSpan timeoutTimeSpan = TimeSpan.FromMilliseconds(User.RealtimeRateMs * 3);

            // For now this is only used for connection timeout
            bool timeout = false;
            byte[]? buffer;
            try
            {
                do
                {
                    (timeout, buffer) = await AsyncFunctions.ExecuteWithTimeout(ReceiveBytes(), timeoutTimeSpan);
                    string message = Encoding.UTF8.GetString(buffer ?? new byte[1]);

                } while (!timeout && ConnectionIsActive);
            }
            catch (Exception ex)
            {
                errorMessage = $"Websocket connection for user {User.Name} with connection id {ConnectionId} threw this exeption: {ex.Message}!";
            }

            if (timeout)
            {
                errorMessage = $"Websocket connection for user {User.Name} with connection id {ConnectionId} timed out!";
            }
            
            Console.WriteLine(errorMessage);
            await CloseConnection(errorMessage);
        }

        public async Task<byte[]> ReceiveBytes()
        {
            var buffer = new byte[1024 * 4];            
            WebSocketReceiveResult result = await WebSocketInstance.ReceiveAsync(buffer, CancellationToken.None);
            return buffer;
        }


        public async Task SendBytesContinuously(byte[]? bytes = null)
        {
            Random rng = new();

            do
            {
                await SendBytes(Encoding.UTF8.GetBytes(rng.Next(0, 11).ToString()));
                await Task.Delay(User.RealtimeRateMs);
            } while (ConnectionIsActive);

            await CloseConnection();
        }

        public async Task SendBytes(byte[] bytes)
        {
            try                
            {
                await WebSocketInstance.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception catched when sending websocket data!! " + ex.Message);
            }
        }

        public async Task CloseConnection(string reason = "")
        {
            if (WebSocketInstance != null && WebSocketInstance.State != WebSocketState.Closed)
            {
                await WebSocketInstance.CloseAsync(
                    closeStatus: WebSocketCloseStatus.NormalClosure,
                    statusDescription: reason == "" ? "Closed by the WebSocketClientManager no reason provided." : reason,
                    cancellationToken: CancellationToken.None);

                string username = User == null ? "Unknown" : User.Name;
                Console.WriteLine($"Connection id {ConnectionId} disconnected! User: {username}");
            }
        }

        public static async Task<(TokenMessageResult status, string token)> AwaitTokenMessage(WebSocket webSocket)
        {
            // Token is expected to be sent on first contact
            var buffer = new byte[1024 * 32];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            // Message contains some characters that must be removed before validation
            message = message.Replace("\"", "");
            message = message.Replace("\0", "");

            string tokenIdentifier = "AuthToken: ";
            string token;

            if (message.StartsWith(tokenIdentifier))
                token = message.Replace(tokenIdentifier, "");
            else
            {
                return (TokenMessageResult.Invalid, "");
            }


            return (TokenMessageResult.Success, token);
        }
    }

    public enum TokenMessageResult
    {
        Success = 0,
        Invalid = -1
    }


}
