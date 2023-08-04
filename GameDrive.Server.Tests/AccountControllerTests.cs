using System.Net;
using System.Net.Http.Json;
using GameDrive.Server.Domain.Models.Requests;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Domain.Models.TransferObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace GameDrive.Server.Tests;

public class AccountControllerTests
{
    private readonly HttpClient _httpClient;

    public AccountControllerTests()
    {
        var sqliteDatabase = SqliteInMemoryDatabase.GetInstance();
        sqliteDatabase.ResetAndRestart();
        
        var webHostBuilder = new WebHostBuilder()
            .UseDevelopmentConfiguration()
            .UseTestSqliteInMemoryDatabase()
            .UseStartup<TestStartup>();
        
        var server = new TestServer(webHostBuilder);
        _httpClient = server.CreateClient();
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
}