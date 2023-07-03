using GameDrive.Server.Domain.Database;
using GameDrive.Server.Models.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Tests;

public class SqliteInMemoryDatabase : IDisposable
{
    private const string ConnectionString = "Data Source=GameDriveTestDb;Mode=Memory;Cache=Shared";
    private static readonly SqliteConnection Connection = new SqliteConnection(ConnectionString);

    public void ResetAndRestart()
    {
        Connection.Close();
        Connection.Open();
    }

    public void RegisterTestDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GameDriveDbContext>));
        if (descriptor != null)
            services.Remove(descriptor);
        
        services.AddDbContext<GameDriveDbContext>(options =>
        {
            options.UseSqlite(ConnectionString, x => x.MigrationsAssembly(DatabaseProvider.Sqlite.Assembly));    
        });
        
    }
    
    public void Dispose() => Connection.Dispose();
    
}

public class GameDriveTestDbContext : GameDriveDbContext
{
    public GameDriveTestDbContext(DbContextOptions options) : base(options)
    {
    }
}