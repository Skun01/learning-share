using Application.Settings;

namespace API.Extensions;

public static class OptionSettingsExtension
{
    public static IServiceCollection AddOptionSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
        .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
