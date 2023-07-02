using GameDrive.Server;
using GameDrive.Server.Extensions;
using GameDrive.Server.Models.Options;
using Microsoft.AspNetCore;

public class Program
{
    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
    }
    
    public static void Main(string[] args)
    {
        var app = BuildWebHost(args);
        app.Run();
    }
}