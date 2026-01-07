using Microsoft.OpenApi.Models;

namespace GestionBD.API.Extensions;

public static class PresentationDependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
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