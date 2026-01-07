using GestionBD.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GestionBD.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR (CQRS)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

        // Servicios de aplicación
        services.AddScoped<EntregableDeploymentService>();

        return services;
    }
}