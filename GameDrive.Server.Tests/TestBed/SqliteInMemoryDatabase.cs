using GameDrive.Server.Domain.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameDrive.Server.Tests;

public class SqliteInMemoryDatabase
{
    private static SqliteInMemoryDatabase? _instance;
    
    private const string ConnectionString = "Data Source=GameDriveTestDb;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _connection = new SqliteConnection(ConnectionString);

    public void ResetAndRestart()
    {
        _connection.Close();
        _connection.Open();
    }

    public static void RegisterTestDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GameDriveDbContext>));
        if (descriptor != null)
            services.Remove(descriptor);
        
        services.AddDbContext<GameDriveDbContext>(options =>
        {
            options.UseSqlite(ConnectionString, x => x.MigrationsAssembly(DatabaseProvider.Sqlite.Assembly));    
        });
        
    }
    
    public static SqliteInMemoryDatabase GetInstance()
    {
        _instance ??= new SqliteInMemoryDatabase();
        return _instance;
    }
}

public class GameDriveTestDbContext : GameDriveDbContext
{
    public GameDriveTestDbContext(DbContextOptions options) : base(options)
    {
    }
}