using GameDrive.Server.Domain.Database;
using GameDrive.Server.Models.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Tests;

public class TestStartup : Startup
{
    public TestStartup(IWebHostEnvironment environment) : base(environment)
    {
    }
    
    public override IConfigurationBuilder CreateConfigurationBuilder(IWebHostEnvironment environment)
    {
        return new ConfigurationBuilder()
            .SetBasePath(environment.ContentRootPath)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddTransient<IOptions<AwsOptions>>((_) => new OptionsWrapper<AwsOptions>(new AwsOptions()
        {
            AccessKey = "", 
            SecretAccessKey = "",
            BucketName = "",
        }));
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);
    }
}