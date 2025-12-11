using Application.DTOs.UserSettings;

namespace Application.DTOs.User;

public record class UserProfileDTO(
    int Id,
    string Username,
    string Email,
    string Role,
    string? AvatarUrl,
    UserSettingsDTO settings
);