using Domain.Enums;

namespace Application.DTOs.UserSettings;

public record class UserSettingsDTO(
    bool EnableGhostMode,
    int DailyGoal,
    string UiLanguage
);
