namespace GameDrive.Server.Domain.Models.Requests;

public record CreateUserRequest(
    string Username,
    string PasswordHash
);