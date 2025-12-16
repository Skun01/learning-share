using Application.DTOs.User;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Domain.Constants;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDTO> GetProfileAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if(user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);

        if(settings == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        return user.ToProfileDTO(settings);
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        var isUsernameExist = await _unitOfWork.Users.AnyAsync(u => u.Id != userId && u.Username == request.Username);
        if (isUsernameExist)
            throw new ApplicationException(MessageConstant.UserMessage.USERNAME_ALREADY_EXISTSS);

        user.Username = request.Username;

        _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
