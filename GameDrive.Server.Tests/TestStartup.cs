using GameDrive.Server.Domain.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GameDrive.Server.Tests;

public class TestStartup : Startup
{
    public TestStartup(IWebHostEnvironment environment) : base(environment)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDbContext<GameDriveDbContext, GameDriveTestDbContext>();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);
    }
}