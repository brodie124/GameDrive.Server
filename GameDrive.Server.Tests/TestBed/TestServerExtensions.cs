using GameDrive.Server.Models;
using GameDrive.Server.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GameDrive.Server.Tests.TestBed;

public static class TestServerExtensions
{
    public static string CreateAuthenticationToken(this TestServer server)
    {
        // var jwtOptions = server.Services.GetRequiredService<JwtOptions>();
        var authenticationService = server.Services.GetRequiredService<AuthenticationService>();
        var jwtToken = authenticationService.CreateToken(new JwtData(
            UserId: 1,
            Username: "someusername"
        ));

        return jwtToken;
    }
}