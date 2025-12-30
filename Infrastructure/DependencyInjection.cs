using Application.IRepositories;
using Application.IServices;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.InternalServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.IServices.IInternal;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructures(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();
        services.AddScoped<IDeckService, DeckService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IStudyService, StudyService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ICramService, CramService>();

        // internal services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailSenderService, EmailSenderService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        
        return services;
    }
}
