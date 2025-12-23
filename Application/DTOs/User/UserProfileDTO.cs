using Application.DTOs.UserSettings;

namespace Application.DTOs.User;

public class UserProfileDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public UserSettingsDTO Settings { get; set; } = null!;
}