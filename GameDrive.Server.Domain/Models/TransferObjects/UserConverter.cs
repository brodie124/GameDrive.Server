namespace GameDrive.Server.Domain.Models.TransferObjects;

public static class UserConverter
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            Username = user.Username
        };
    }
    
    public static User ToUser(this UserDto userDto)
    {
        return new User()
        {
            Id = userDto.Id,
            Username = userDto.Username,
            PasswordHash = string.Empty
        };
    }
}