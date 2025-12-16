namespace Application.Settings;

public class AppSettings
{
    public const string SectionName = "AppSettings";
    public string Name { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public string ResetPasswordUrl { get; set; } = string.Empty;
}
