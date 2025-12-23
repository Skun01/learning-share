using Domain.Enums;

namespace Application.DTOs.UserSettings;

public class UserSettingsDTO
{
    public bool EnableGhostMode { get; set; }
    public int DailyGoal { get; set; }
    public string UiLanguage { get; set; } = string.Empty;
}
