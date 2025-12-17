using Application.DTOs.User;
using Microsoft.AspNetCore.Http;

namespace Application.IServices;

public interface IUserService
{
    Task<UserProfileDTO> GetProfileAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<string> UploadAvatarAsync(int userId, IFormFile file);
    Task<bool> ChangeUserPasswordAsync(int userId, ChangePasswordRequest request);
}
