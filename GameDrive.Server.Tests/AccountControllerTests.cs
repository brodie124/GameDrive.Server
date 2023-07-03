using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using GameDrive.Server.Controllers;
using GameDrive.Server.Domain.Database;
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
    const string ConnectionString = "Data Source=GameDriveTestDb;Mode=Memory;Cache=Shared";
    
    // private readonly SqliteInMemoryDatabase _sqliteMemoryDatabase;
    // private readonly GameDriveDbContext _gameDriveDatabaseContext;
    private readonly TestServer _server;
    private readonly HttpClient _httpClient;
    private static SqliteConnection? _sqliteConnection;

    public AccountControllerTests()
    {
        var projectDir = GetProjectPath("", typeof(AccountControllerTests).GetTypeInfo().Assembly);
        var webHostBuilder = new WebHostBuilder()
            .UseEnvironment("Development")
            .UseContentRoot(projectDir)
            .UseConfiguration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build()
            )
            .ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GameDriveDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                
                _sqliteConnection = new SqliteConnection(ConnectionString);
                _sqliteConnection.Open();
                services.AddDbContext<GameDriveDbContext>(options =>
                {
                    options.UseSqlite(
                        ConnectionString,
                        x => x.MigrationsAssembly(DatabaseProvider.Sqlite.Assembly)
                    );
                });

            })
            .UseStartup<Startup>();
            

        _server = new TestServer(webHostBuilder);
        _httpClient = _server.CreateClient();
    }
    
    [Fact]
    public async void SignUp_WithValidData_ShouldReturnOK()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("test_user", "test_password");

        // Act
        var response = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);

        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(apiResponse?.IsSuccess);
        Assert.Equal("test_user", apiResponse?.Data?.Username);
    }
    
    
    [Fact]
    public async void SignUp_WithDuplicateUsername_ShouldReturnApiError()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest("test_user", "test_password");

        // Act
        var responseOne = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);
        var responseTwo = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);

        responseOne.EnsureSuccessStatusCode();
        responseTwo.EnsureSuccessStatusCode();
        
        var apiResponseOne = await responseOne.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        var apiResponseTwo = await responseTwo.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

        // Assert
        Assert.True(apiResponseOne?.IsSuccess);
        Assert.False(apiResponseTwo?.IsSuccess);
        Assert.Equal("test_user", apiResponseOne?.Data?.Username);
        Assert.Equal(ApiResponseCode.AuthUsernameTaken, apiResponseTwo?.ResponseCode);
    }
    
    [Fact]
    public async void LogIn_WithValidData_ShouldReturnOK()
    {
        // Arrange
        const string username = "test_user";
        const string password = "test_password";
        var createUserRequest = new CreateUserRequest(username, password);

        // Act
        var signUpResponse = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);
        var logInResponse = await _httpClient.PostAsJsonAsync($"Account/LogIn?username={username}&passwordHash={password}", string.Empty);

        signUpResponse.EnsureSuccessStatusCode();
        logInResponse.EnsureSuccessStatusCode();
        
        var signUpDto = await signUpResponse.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        var logInDto = await logInResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();

        // Assert
        Assert.True(signUpDto?.IsSuccess);
        Assert.True(logInDto?.IsSuccess);
        Assert.NotNull(logInDto?.Data);
        Assert.True(logInDto?.Data?.Length > 0);
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