using System.Security.Claims;

namespace VisualHomeBackend.Models
{
    public static class UserClaims
    {
        public static string NameType { get; } = "name";
        public static string GetNameValue(IEnumerable<Claim> claims) { return claims.FirstOrDefault(claim => claim.Type == NameType)?.Value ?? ""; }
    }
}
