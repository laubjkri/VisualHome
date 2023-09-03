using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text;
using VisualHomeBackend.Models;

namespace VisualHomeBackend.Services
{
    public class UserWsConnectionManagerService
    {
        private readonly ConcurrentDictionary<string, UserWsConnection> _userConnections;        

        public UserWsConnectionManagerService()
        {
            _userConnections = new ConcurrentDictionary<string, UserWsConnection>();            
        }

        public void AddConnection(UserWsConnection userWsConnection)
        {
            _userConnections[userWsConnection.ConnectionId] = userWsConnection;
        }
    }
}
