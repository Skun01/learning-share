namespace Application.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpireHours { get; init; } = 60;
    public int RefreshTokenExpireDays { get; init; }
}
