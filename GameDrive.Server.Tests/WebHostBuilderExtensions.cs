// using System.Reflection;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace GameDrive.Server.Tests;
//
// internal static class WebHostBuilderExtensions
// {
//     public static IWebHostBuilder UseStartupInstance(this IWebHostBuilder hostBuilder, IStartup startup)
//     {
//         string name = startup.GetType().GetTypeInfo().Assembly.GetName().Name;
//         return hostBuilder.UseSetting(WebHostDefaults.ApplicationKey, name)
//             .ConfigureServices((services =>
//             {
//                 if (typeof(IStartup).GetTypeInfo().IsAssignableFrom(startup.GetType().GetTypeInfo()))
//                     services.AddSingleton(startup);
//                 else
//                     services.AddSingleton(typeof(IStartup), serviceProvider =>
//                     {
//                         var requiredService = serviceProvider.GetRequiredService<IWebHostEnvironment>();
//                         return new ConventionBasedStartup(StartupLoader.LoadMethods(serviceProvider, startup.GetType(),
//                             requiredService.EnvironmentName));
//                     });
//             }));
//     }
// }