using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Requests;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Domain.Models.TransferObjects;
using GameDrive.Server.Tests.TestBed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GameDrive.Server.Tests;

public class ManifestControllerTests
{
    private readonly string _jwtToken;
    private readonly HttpClient _httpClient;
    private readonly DateTime _startDate = new DateTime(2023, 01, 01, 18, 00, 00);
    
    public ManifestControllerTests()
    {
        var sqliteDatabase = SqliteInMemoryDatabase.GetInstance();
        sqliteDatabase.ResetAndRestart();

        var webHostBuilder = new WebHostBuilder()
                .SetupTestEnvironment(sqliteDatabase);

        var server = new TestServer(webHostBuilder);
        _httpClient = server.CreateClient();
        _jwtToken = server.CreateAuthenticationToken();
    }

    [Fact]
    public async void Compare_WithNoFiles_ShouldReturnEmptyReport()
    {
        // Arrange
        var compareManifestRequest = new CompareManifestRequest
        {
            Manifest = new GameProfileManifest
            {
                GameProfileId = "unknown-game-id",
                Entries = new ManifestEntry[]
                {
                    new()
                    {
                        Guid = Guid.NewGuid(),
                        CreatedDate = _startDate.Add(TimeSpan.Zero),
                        LastModifiedDate = _startDate.Add(TimeSpan.FromMinutes(10)),
                        RelativePath = "save.abc",
                        FileHash = "123",
                        FileSize = 1250,
                        IsDeleted = false,
                    }
                }
            }.ToDto()
        };

        var json = JsonSerializer.Serialize(compareManifestRequest);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "Manifest/Compare");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        httpRequest.Content = httpContent;
        

        // Act
        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CompareManifestResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(apiResponse?.IsSuccess);
        // Assert.Equal("test_user", "");
    }


    // [Fact]
    // public async void SignUp_WithDuplicateUsername_ShouldReturnApiError()
    // {
    //     // Arrange
    //     var createUserRequest = new CreateUserRequest("test_user", "test_password");
    //
    //     // Act
    //     var responseOne = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);
    //     var responseTwo = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);
    //     responseOne.EnsureSuccessStatusCode();
    //     responseTwo.EnsureSuccessStatusCode();
    //
    //     var apiResponseOne = await responseOne.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
    //     var apiResponseTwo = await responseTwo.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
    //
    //     // Assert
    //     Assert.True(apiResponseOne?.IsSuccess);
    //     Assert.False(apiResponseTwo?.IsSuccess);
    //     Assert.Equal("test_user", apiResponseOne?.Data?.Username);
    //     Assert.Equal(ApiResponseCode.AuthUsernameTaken, apiResponseTwo?.ResponseCode);
    // }
    //
    // [Fact]
    // public async void LogIn_WithValidData_ShouldReturnOK()
    // {
    //     // Arrange
    //     const string username = "test_user";
    //     const string password = "test_password";
    //     var createUserRequest = new CreateUserRequest(username, password);
    //
    //     // Act
    //     var signUpResponse = await _httpClient.PostAsJsonAsync("Account/SignUp", createUserRequest);
    //     var logInResponse =
    //         await _httpClient.PostAsJsonAsync($"Account/LogIn?username={username}&passwordHash={password}",
    //             string.Empty);
    //
    //     signUpResponse.EnsureSuccessStatusCode();
    //     logInResponse.EnsureSuccessStatusCode();
    //
    //     var signUpDto = await signUpResponse.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
    //     var logInDto = await logInResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
    //
    //     // Assert
    //     Assert.True(signUpDto?.IsSuccess);
    //     Assert.True(logInDto?.IsSuccess);
    //     Assert.NotNull(logInDto?.Data);
    //     Assert.True(logInDto?.Data?.Length > 0);
    // }
}