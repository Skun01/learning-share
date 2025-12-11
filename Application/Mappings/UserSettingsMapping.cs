using Application.DTOs.UserSettings;
using Domain.Entities;

namespace Application.Mappings;

public static class UserSettingsMapping
{
    public static UserSettingsDTO ToDTO(this UserSettings entity)
    {
        return new UserSettingsDTO(
            entity.EnableGhostMode,
            entity.DailyGoal,
            entity.UiLanguage.ToString()
        );
    }
}
