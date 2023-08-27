using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Dynamic;
using System.Text;
using System.Text.Json;

namespace VisualHomeBackend.Services
{
    public class JwtParameterFetcher
    {
        private class JwtParameter
        {
            public string Key { get; set; } = "";
            public string Issuer { get; set; } = "";
            public string Audience { get; set; } = "";
        }

        private string _filePath = "JWTparameters.json";
        private string _json = "";
        private TokenValidationParameters _validationParameters;
        private SigningCredentials _signingCredentials;

        // Singleton data
        private static JwtParameterFetcher? _instance;
        private static readonly object _instanceCreationLock = new object();

        private JwtParameterFetcher()
        {           

            if (!File.Exists(_filePath)) throw new FileNotFoundException($"{_filePath} file not found.");
            _json = File.ReadAllText(_filePath);
            
            
            JwtParameter jwtParameter = JsonSerializer.Deserialize<JwtParameter>(_json) ?? throw new Exception($"Deserialization failed with null.");
            var keyString = jwtParameter.Key;
            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            var key = new SymmetricSecurityKey(keyBytes);             

            TokenValidationParameters tokenValidationParameters = new()
            {
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
            
            _validationParameters = tokenValidationParameters;

            // For the token generator:
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private static void CreateInstanceIfNull()
        {
            if (_instance == null)
            {
                lock (_instanceCreationLock)
                {
                    if ( _instance == null )
                    {
                        _instance = new JwtParameterFetcher();
                    }
                }
            }            
        }

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            CreateInstanceIfNull();
            return _instance!._validationParameters;
        }

        public static SigningCredentials GetTokenSigningCredentials()
        {
            CreateInstanceIfNull();
            return _instance!._signingCredentials;
        }



    }
}
