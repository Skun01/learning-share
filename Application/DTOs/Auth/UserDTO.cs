using Domain.Enums;

namespace Application.DTOs.Auth;

public record class UserDTO(
    int Id,
    string Username,
    string Email,
    UserRole Role,
    string AvatarUrl
);
