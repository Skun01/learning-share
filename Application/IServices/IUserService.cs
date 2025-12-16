using Application.DTOs.User;

namespace Application.IServices;

public interface IUserService
{
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request);
}
