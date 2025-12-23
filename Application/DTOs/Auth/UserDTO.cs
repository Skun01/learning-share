using Domain.Enums;

namespace Application.DTOs.Auth;

public class UserDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}
