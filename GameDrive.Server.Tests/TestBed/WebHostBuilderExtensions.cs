using GameDrive.Server.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Tests.TestBed;

internal static class WebHostBuilderExtensions
{
    public static IWebHostBuilder SetupTestEnvironment(
        this IWebHostBuilder hostBuilder,
        SqliteInMemoryDatabase sqliteInMemoryDatabase
    )
    {
        return hostBuilder
            .UseStartup<Startup>()
            .UseDevelopmentConfiguration()
            .UseTestSqliteInMemoryDatabase(sqliteInMemoryDatabase)
            .RegisterTestServices();
    }
    
    public static IWebHostBuilder UseDevelopmentConfiguration(this IWebHostBuilder hostBuilder)
    {
        return hostBuilder
            .UseEnvironment("Development")
            .UseConfiguration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build()
            );
    }
    public static IWebHostBuilder UseTestSqliteInMemoryDatabase(this IWebHostBuilder hostBuilder, SqliteInMemoryDatabase sqliteInMemoryDatabase)
    {
        return hostBuilder
            .ConfigureTestServices(sqliteInMemoryDatabase.RegisterTestDbContext);
    }

    public static IWebHostBuilder RegisterTestServices(this IWebHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureTestServices(services =>
        {
            services.AddTransient<IOptions<AwsOptions>>((_) => new OptionsWrapper<AwsOptions>(new AwsOptions()
            {
                AccessKey = "", 
                SecretAccessKey = "",
                BucketName = "",
            }));
        });
    }
}