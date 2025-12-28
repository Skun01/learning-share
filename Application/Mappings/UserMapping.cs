using Application.DTOs.Auth;
using Application.DTOs.User;
using Domain.Entities;

namespace Application.Mappings;

public static class UserMapping
{
    public static User ToEntityFromRegisterRequest(this RegisterRequest request)
    {
        return new User
        {
            Username = request.Username,
            Email = request.Email
        };
    }

    public static UserDTO ToDTO(this User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            AvatarUrl = user.AvatarMedia != null ? "/" + user.AvatarMedia.FilePath : string.Empty
        };
    }

    public static UserProfileDTO ToProfileDTO(this User user, UserSettings settings)
    {
        return new UserProfileDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            AvatarUrl = user.AvatarMedia != null ? "/" + user.AvatarMedia.FilePath : null,
            Settings = settings.ToDTO()
        };
    }
}
