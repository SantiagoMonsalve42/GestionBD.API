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
                Description = "API para gestión de bases de datos con CQRS"
            });
        });

        return services;
    }
}