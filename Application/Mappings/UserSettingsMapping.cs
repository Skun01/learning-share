using Application.DTOs.UserSettings;
using Domain.Entities;

namespace Application.Mappings;

public static class UserSettingsMapping
{
    public static UserSettingsDTO ToDTO(this UserSettings entity)
    {
        return new UserSettingsDTO
        {
            EnableGhostMode = entity.EnableGhostMode,
            DailyGoal = entity.DailyGoal,
            UiLanguage = entity.UiLanguage.ToString()
        };
    }
}
