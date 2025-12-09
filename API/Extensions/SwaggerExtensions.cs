using System.Reflection;
using Microsoft.OpenApi.Models;

namespace API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerInfrastructure(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {   
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Learn language project",
                Version = "v1",
                Description = "API document for my project",
                Contact = new OpenApiContact
                {
                    Name = "Thai Truong",
                    Email = "truongg9655@gmail.com"
                }
            });

            // configure comment XML
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            // Cấu hình bảo mật (JWT Bearer) - Chuẩn bị cho chức năng Login sau này
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Nhập 'Bearer [your-token]' vào ô bên dưới",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }
}
