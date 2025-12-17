using Application.DTOs.UserSettings;

namespace Application.IServices;

public interface IUserSettingsService
{
    Task<UserSettingsDTO> GetSettingsAsync(int userId);
    Task<bool> UpdateGhostModeAsync(int userId, UpdateGoshModeRequest request);
    Task<bool> UpdateDailyGoalAsync(int userId, UpdateDailyGoalRequest request);
    Task<bool> UpdateUiLanguageAsync(int userId, UpdateLanguageRequest request);
}
