namespace Application.DTOs.Auth;

public record class AuthDTO(
    string AccessToken,
    string RefreshToken,
    UserDTO User
);
