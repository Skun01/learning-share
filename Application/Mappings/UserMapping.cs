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
        return new UserDTO(
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            user.AvatarUrl ?? string.Empty
        );
    }

    public static UserProfileDTO ToProfileDTO(this User user, UserSettings settings)
    {
        return new UserProfileDTO(
            user.Id,
            user.Username,
            user.Email,
            user.Role.ToString(),
            user.AvatarUrl,
            settings.ToDTO()
        );
    }
}
