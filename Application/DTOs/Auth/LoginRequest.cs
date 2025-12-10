namespace Application.DTOs.Auth;

public record class LoginRequest(
    string Email,
    string Password
);
