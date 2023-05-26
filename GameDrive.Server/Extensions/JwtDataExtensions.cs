using System.IdentityModel.Tokens.Jwt;
using GameDrive.Server.Models;
using Microsoft.Net.Http.Headers;

namespace GameDrive.Server.Extensions;

public static class JwtDataExtensions
{
    public static JwtData GetJwtDataFromRequest(this HttpRequest request)
    {
        var authHeader = request.Headers[HeaderNames.Authorization];
        var jwtString = authHeader
            .ToString()
            .Replace("Bearer", "")
            .Trim();

        var jwt = new JwtSecurityToken(jwtEncodedString: jwtString);
        return JwtData.FromClaims(jwt.Claims);
    }
}