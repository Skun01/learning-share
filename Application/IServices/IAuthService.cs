using Application.DTOs.Auth;
using Application.DTOs.User;

namespace Application.IServices;

public interface IAuthService
{
    Task<AuthDTO> RegisterAsync(RegisterRequest request);
    Task<AuthDTO> LoginAsync(LoginRequest request);
    Task<UserProfileDTO> GetProfileAsync(int userId);
}
