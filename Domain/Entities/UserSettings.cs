using Domain.Enums;

namespace Domain.Entities;

public class UserSettings
{
    public int UserId { get; set; }
    public bool EnableGhostMode { get; set; } = true;
    public int DailyGoal { get; set; } = 10;
    public UiLanguage UiLanguage { get; set; } = UiLanguage.Vi;
    public User User { get; set; } = null!;
}
