using Application.DTOs.UserSettings;
using Application.IRepositories;
using Application.IServices;
using Application.Mappings;
using Domain.Constants;
using Domain.Enums;

namespace Application.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IUnitOfWork _unitOfWork;
    public UserSettingsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserSettingsDTO> GetSettingsAsync(int userId)
    {
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if(settings == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        return settings.ToDTO();
    }

    public async Task<bool> UpdateDailyGoalAsync(int userId, UpdateDailyGoalRequest request)
    {
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if (settings == null) 
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        settings.DailyGoal = request.Goal;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateGhostModeAsync(int userId, UpdateGoshModeRequest request)
    {
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if(settings == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        settings.EnableGhostMode = request.Enabled;

        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> UpdateUiLanguageAsync(int userId, UpdateLanguageRequest request)
    {
        var settings = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if(settings == null)
            throw new ApplicationException(MessageConstant.CommonMessage.NOT_FOUND);

        if (Enum.TryParse<UiLanguage>(request.Language, true, out var parsedLang))
        {
            settings.UiLanguage = parsedLang;
            await _unitOfWork.SaveChangesAsync();
        }else
            throw new ApplicationException(MessageConstant.UserSettingsMessage.NOT_SUPPORT_LANGUAGE);

        return true;
    }
}
