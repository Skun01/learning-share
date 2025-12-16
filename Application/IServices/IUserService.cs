using Application.DTOs.User;

namespace Application.IServices;

public interface IUserService
{
    Task<UserProfileDTO> GetProfileAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request);
}
