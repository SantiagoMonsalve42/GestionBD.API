using GestionBD.API.Configuration;
using Microsoft.OpenApi.Models;

namespace GestionBD.API.Extensions;

public static class PresentationDependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar CORS desde appsettings
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>()
            ?? throw new InvalidOperationException("CORS configuration is missing in appsettings.json");

        services.AddCors(options =>
        {
            options.AddPolicy(corsSettings.PolicyName, builder =>
            {
                builder.WithOrigins(corsSettings.AllowedOrigins)
                       .WithMethods(corsSettings.AllowedMethods)
                       .WithHeaders(corsSettings.AllowedHeaders);

                if (corsSettings.AllowCredentials)
                {
                    builder.AllowCredentials();
                }

                if (corsSettings.MaxAge > 0)
                {
                    builder.SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.MaxAge));
                }
            });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GestionBD API",
                Version = "v1",
                Description = "API para gestión de bases de datos con CQRS y autenticación Keycloak"
            });

            // Configuración de seguridad para Keycloak/JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingrese el token JWT obtenido desde Keycloak. Ejemplo: 'Bearer {token}'"
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
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}