using GameDrive.Server.Domain.Models;
using GameDrive.Server.Domain.Models.Requests;
using GameDrive.Server.Domain.Models.Responses;
using GameDrive.Server.Domain.Models.TransferObjects;
using GameDrive.Server.Models;
using GameDrive.Server.Services;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameDrive.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly AuthenticationService _authenticationService;

    public AccountController(
        UserRepository userRepository,
        AuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _authenticationService = authenticationService;
    }
    
    [HttpPost("LogIn")]
    public async ValueTask<ApiResponse<string>> LogIn([FromQuery] string username, [FromQuery] string passwordHash)
    {
        var users = await _userRepository.FindAsync((x =>
            x.Username.Equals(username) &&
            x.PasswordHash == passwordHash));
        if (!users.Any())
        {
            return ApiResponse<string>.Failure("Invalid username/password provided.", ApiResponseCode.AuthInvalidCredentials);
        }

        var userDto = users.First().ToDto();
        var jwtToken = _authenticationService.CreateToken(new JwtData(
            UserId: userDto.Id,
            Username: userDto.Username
        ));
        return ApiResponse<string>.Success(jwtToken);
    }

    [HttpPost("SignUp")]
    public async ValueTask<ApiResponse<User>> SignUp([FromBody] CreateUserRequest createUserRequest)
    {
        var existingUsers = await _userRepository.FindAsync(x =>
            x.Username.Equals(createUserRequest.Username));
        if (existingUsers.Any())
        {
            return ApiResponse<User>.Failure("The username provided is already taken.",
                ApiResponseCode.AuthUsernameTaken);
        }

        var user = new User()
        {
            Username = createUserRequest.Username,
            PasswordHash = createUserRequest.PasswordHash,
            StorageObjects = new List<StorageObject>()
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return user;
    }
}