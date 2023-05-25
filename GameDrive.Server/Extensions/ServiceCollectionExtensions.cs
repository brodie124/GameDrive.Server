using System.Text;
using GameDrive.Server.Database;
using GameDrive.Server.Models.Options;
using GameDrive.Server.Services;
using GameDrive.Server.Services.Repositories;
using GameDrive.Server.Services.Storage;
using GameDrive.Server.Tasks.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GameDrive.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameDriveServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IStorageProvider, LocalStorageProvider>();
        serviceCollection.AddScoped<StorageService>();
        serviceCollection.AddScoped<AuthenticationService>();
        serviceCollection.AddScoped<StorageObjectRepository>();
        serviceCollection.AddScoped<UserRepository>();
        serviceCollection.AddHostedService<MigrateDatabaseTask>();
        return serviceCollection;
    }
    
    public static IServiceCollection AddGameDriveDbContext(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<GameDriveDbContext>();
        return serviceCollection;
    }
    
    public static IServiceCollection AddGameDriveConfigurationOptions(
        this IServiceCollection serviceCollection, 
        IConfiguration configuration
    )
    {
        serviceCollection.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        serviceCollection.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        return serviceCollection;
    }
    
    // TODO: update this function to use values pulled from the configuration file
    public static IServiceCollection AddGameDriveAuthentication(this IServiceCollection serviceCollection, JwtOptions jwtOptions)
    {
        serviceCollection
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)
                    )
                };
            });

        return serviceCollection;
    }
    
}