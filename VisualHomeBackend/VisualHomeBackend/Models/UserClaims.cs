using System.Security.Claims;

namespace VisualHomeBackend.Models
{
    public static class UserClaims
    {
        public static string NameType { get; } = "name";
        public static string IdType { get; } = "id";
        public static string? GetValueOfClaimType(IEnumerable<Claim> claims, string claimType) { return claims.FirstOrDefault(claim => claim.Type == claimType)?.Value; }
    }
}
