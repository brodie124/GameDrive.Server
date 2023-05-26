using System.Security.Claims;

namespace GameDrive.Server.Models;

public record JwtData(
    int UserId,
    string Username
) 
{
    public IEnumerable<Claim> CreateClaims()
    {
        return new Claim[]
        {
            new(ClaimTypes.Name, Username),
            new(ClaimTypes.NameIdentifier, UserId.ToString()),
            new(ClaimTypes.Role, "user"),
        };
    }

    public static JwtData FromClaims(IEnumerable<Claim> claims)
    {
        var claimsArr = claims.ToList();
        var userIdString = claimsArr.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var username = claimsArr.First(x => x.Type == ClaimTypes.Name).Value;
        if (!int.TryParse(userIdString, out var userId))
            throw new ArgumentException("JWT UserId must be of type int!");

        return new JwtData(
            UserId: userId,
            Username: username
        );
    }
}