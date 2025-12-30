using Application.DTOs.Auth;
using Application.DTOs.User;

namespace Application.IServices;

public interface IAuthService
{
    Task<AuthDTO> RegisterAsync(RegisterRequest request);
    Task<AuthDTO> LoginAsync(LoginRequest request);
    Task<bool> SendResetPasswordEmailAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task<AuthDTO> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> RevokeTokenAsync(string refreshToken);
}
