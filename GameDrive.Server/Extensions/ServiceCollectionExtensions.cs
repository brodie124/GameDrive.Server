using System.Text;
using GameDrive.Server.Domain.Database;
using GameDrive.Server.Models.Options;
using GameDrive.Server.Services;
using GameDrive.Server.Services.Repositories;
using GameDrive.Server.Services.Storage;
using GameDrive.Server.Tasks;
using GameDrive.Server.Tasks.Repeating;
using GameDrive.Server.Tasks.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameDrive.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameDriveServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IRepeatingTask, TriggerReplicationRepeatingTask>();
        
        serviceCollection.AddScoped<TemporaryStorageProvider>();
        serviceCollection.AddScoped<ICloudStorageProvider, LocalCloudStorageProvider>();
        serviceCollection.AddScoped<IStorageReplicationService, StorageReplicationService>();
        serviceCollection.AddScoped<StorageService>();
        serviceCollection.AddScoped<AuthenticationService>();
        serviceCollection.AddScoped<ManifestService>();
        serviceCollection.AddScoped<IStorageObjectRepository, StorageObjectRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IBucketRepository, BucketRepository>();
        serviceCollection.AddSingleton<IRepeatingTaskService, RepeatingTaskService>();
        serviceCollection.AddHostedService<MigrateDatabaseTask>();
        serviceCollection.AddHostedService<InitialiseRepeatingTaskService>();
        return serviceCollection;
    }
    
    public static IServiceCollection AddGameDriveDbContext(
        this IServiceCollection serviceCollection, 
        IConfiguration configuration,
        DatabaseOptions databaseOptions
    )
    {
        serviceCollection.AddDbContext<GameDriveDbContext>(options =>
        {
            var provider = configuration.GetValue("provider", DatabaseProvider.Mysql.Name).ToLower();
            if (provider == DatabaseProvider.Mysql.Name.ToLower()) {
                var serverVersion = MySqlServerVersion.AutoDetect(databaseOptions.MysqlConnectionString);
                options.UseMySql(databaseOptions.MysqlConnectionString, serverVersion,
                    x => x.MigrationsAssembly(DatabaseProvider.Mysql.Assembly));
            }
            
            if (provider == DatabaseProvider.Sqlite.Name.ToLower())
            {
                options.UseSqlite(
                    databaseOptions.SqliteConnectionString,
                    x => x.MigrationsAssembly(DatabaseProvider.Sqlite.Assembly)
                );
            }

            options.EnableSensitiveDataLogging(false);
        });
        
        return serviceCollection;
    }
    
    public static IServiceCollection AddGameDriveConfigurationOptions(
        this IServiceCollection serviceCollection, 
        IConfiguration configuration
    )
    {
        serviceCollection.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        serviceCollection.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        serviceCollection.Configure<TemporaryStorageOptions>(configuration.GetSection(TemporaryStorageOptions.SectionName));
        return serviceCollection;
    }
    
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


