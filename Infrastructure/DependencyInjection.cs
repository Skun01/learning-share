using Application.IRepositories;
using Application.IServices;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
