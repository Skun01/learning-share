using Application.DTOs.User;
using Application.IRepositories;
using Application.IServices;
using Application.IServices.IInternal;
using Application.Mappings;
using Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    public UserService(IUnitOfWork unitOfWork, IStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _storageService = storageService;
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

    public async Task<string> UploadAvatarAsync(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ApplicationException(MessageConstant.CommonMessage.INVALID);

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!FileUploadConstant.ALLOW_EXTENSIONS.Contains(extension))
            throw new ApplicationException(MessageConstant.FileUploadMessage.INVALID_EXTENSION);

        if (file.Length > FileUploadConstant.MAX_STORAGE) 
            throw new ApplicationException(MessageConstant.FileUploadMessage.STORAGE_EXCEED);

        var avatarUrl = await _storageService.SaveFileAsync(file, FileUploadConstant.AVATAR_FOLDER);

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        user.AvatarUrl = avatarUrl;
        _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return avatarUrl;
    }
}
