using GameDrive.Server.Database;
using GameDrive.Server.OptionsModels;
using GameDrive.Server.Services.Storage;
using GameDrive.Server.Tasks.Startup;

namespace GameDrive.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameDriveServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IStorageProvider, LocalStorageProvider>();
        serviceCollection.AddScoped<StorageService>();
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
        return serviceCollection;
    }
    
}