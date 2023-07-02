using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using GameDrive.Server.Controllers;
using GameDrive.Server.Database;
using GameDrive.Server.Domain.Models.Requests;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Domain.Models.TransferObjects;
using GameDrive.Server.Models.Options;
using GameDrive.Server.Services;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace GameDrive.Server.Tests;

public class AccountControllerTests
{
    private readonly SqliteInMemoryDatabase _sqliteMemoryDatabase;
    private readonly GameDriveDbContext _gameDriveDatabaseContext;
    private readonly JwtOptions _jwtOptions;
    private readonly TestServer _server;
    private readonly HttpClient _httpClient;

    public AccountControllerTests()
    {
        _sqliteMemoryDatabase = new SqliteInMemoryDatabase();
        _gameDriveDatabaseContext = _sqliteMemoryDatabase.CreateContext();
        _jwtOptions = new JwtOptions()
        {
            Key = "game-drive-test-key-1234-5678",
            Audience = "game-drive-test-audience",
            Issuer = "game-drive-test-issuer"
        };

        var projectDir = GetProjectPath("", typeof(AccountControllerTests).GetTypeInfo().Assembly);

        var webHostBuilder = new WebHostBuilder()
            .UseEnvironment("Development")
            .UseContentRoot(projectDir)
            .UseStartup<Startup>()
            .UseConfiguration(new ConfigurationBuilder()
                // .SetBasePath(projectDir)
                .AddJsonFile("appsettings.Development.json")
                .Build()
            )
            .ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(GameDriveDbContext));
                if (descriptor != null)
                    services.Remove(descriptor);

                var connectionString = "DataSource=game-drive-test-db;mode=memory;cache=shared";
                var connection = new SqliteConnection(connectionString);
                connection.Open();

                services.AddDbContext<GameDriveDbContext>(options =>
                {
                    options.UseSqlite(connectionString);
                });

            });
            

        _server = new TestServer(webHostBuilder);
        _httpClient = _server.CreateClient();
    }
    
    [Fact]
    public async void SignUp_WithValidData_ShouldReturnOK()
    {
        // Arrange
        // var userRepository = new UserRepository(_gameDriveDatabaseContext);
        // var authenticationService = new AuthenticationService(new OptionsWrapper<JwtOptions>(_jwtOptions));
        // var sut = new AccountController(userRepository, authenticationService);
        var createUserRequest = new CreateUserRequest("test_user", "test_password");

        // Act
        var test = await _httpClient.GetAsync("/Account/LogIn?username=test&passwordHash=1234");
        var test2 = await _httpClient.GetAsync("/swagger/v1/swagger.json");
        var test2Content = await test2.Content.ReadAsStringAsync();
        
        var healthResponse = await _httpClient.GetAsync("/Health");
        
        var response = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        Assert.True(apiResponse?.IsSuccess);
        Assert.Equal("test_user", apiResponse?.Data?.Username);
    }
    
    private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
    {
        // Get name of the target project which we want to test
        var projectName = startupAssembly.GetName().Name;

        // Get currently executing test project path
        var applicationBasePath = System.AppContext.BaseDirectory;

        // Find the path to the target project
        var directoryInfo = new DirectoryInfo(applicationBasePath);
        do
        {
            directoryInfo = directoryInfo.Parent;

            var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
            if (projectDirectoryInfo.Exists)
            {
                var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                if (projectFileInfo.Exists)
                {
                    return Path.Combine(projectDirectoryInfo.FullName, projectName);
                }
            }
        }
        while (directoryInfo.Parent != null);

        throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
    }
}