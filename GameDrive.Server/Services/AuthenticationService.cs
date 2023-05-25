using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameDrive.Server.Domain.Models.TransferObjects;
using GameDrive.Server.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GameDrive.Server.Services;

public class AuthenticationService
{
    private readonly JwtOptions _jwtOptions;
    private readonly TimeSpan _tokenLifetimeMinutes = TimeSpan.FromMinutes(60);

    public AuthenticationService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }
    
    public string CreateToken(UserDto userDto)
    {
        var claims = new Claim[]
        {
            new(ClaimTypes.Name, userDto.Username),
            new(ClaimTypes.Role, "user"),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var descriptor = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.Now.Add(_tokenLifetimeMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(descriptor);
    }

    public bool IsTokenValid(string token)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = securityKey,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        return validatedToken is not null;
    }
}