using System.Security.Claims;

namespace GameDrive.Server.Models;

public record JwtData(
    int UserId,
    string Username
) 
{
    public Claim[] CreateClaims()
    {
        return new Claim[]
        {
            new(ClaimTypes.Name, Username),
            new(ClaimTypes.NameIdentifier, UserId.ToString()),
            new(ClaimTypes.Role, "user"),
        };
    }

    public static JwtData FromClaims(Claim[] claims)
    {
        var userIdString = claims.First(x => x.ValueType == ClaimTypes.NameIdentifier).Value;
        var username = claims.First(x => x.ValueType == ClaimTypes.Name).Value;
        if (!int.TryParse(userIdString, out var userId))
            throw new ArgumentException("JWT UserId must be of type int!");

        return new JwtData(
            UserId: userId,
            Username: username
        );
    }
}