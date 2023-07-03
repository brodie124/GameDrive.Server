using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace GameDrive.Server.Tests;

internal static class WebHostBuilderExtensions
{
    public static IWebHostBuilder UseDevelopmentConfiguration(this IWebHostBuilder hostBuilder)
    {
        return hostBuilder
            .UseEnvironment("Development")
            .UseConfiguration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build()
            );
    }
    public static IWebHostBuilder UseTestSqliteInMemoryDatabase(this IWebHostBuilder hostBuilder)
    {
        return hostBuilder
            .ConfigureTestServices(SqliteInMemoryDatabase.RegisterTestDbContext);
    }
}