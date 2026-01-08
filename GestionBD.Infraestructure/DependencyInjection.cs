using GestionBD.Application.Abstractions;
using GestionBD.Application.Configuration;
using GestionBD.Domain;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Repositories.Query;
using GestionBD.Infraestructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Data;

namespace GestionBD.Infraestructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext para Commands (EF Core) - usando IOptions
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionStrings = serviceProvider.GetRequiredService<IOptions<ConnectionStringsSettings>>();
            options.UseSqlServer(connectionStrings.Value.DefaultConnection);
        });

        // IDbConnection para Queries (Dapper) - usando IOptions
        services.AddScoped<IDbConnection>(sp =>
        {
            var connectionStrings = sp.GetRequiredService<IOptions<ConnectionStringsSettings>>();
            return new SqlConnection(connectionStrings.Value.DefaultConnection);
        });

        // Unit of Work (para Commands)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDacpacService, DacpacService>();
        // File Storage Service
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Read Repositories (para Queries - Dapper)
        services.AddScoped<IArtefactoReadRepository, ArtefactoReadRepository>();
        services.AddScoped<IEntregableReadRepository, EntregableReadRepository>();
        services.AddScoped<IEjecucionReadRepository, EjecucionReadRepository>();
        services.AddScoped<IInstanciaReadRepository, InstanciaReadRepository>();
        services.AddScoped<IMotorReadRepository, MotorReadRepository>();
        services.AddScoped<ILogEventoReadRepository, LogEventoReadRepository>();
        services.AddScoped<ILogTransaccionReadRepository, LogTransaccionReadRepository>();
        services.AddScoped<IParametroReadRepository, ParametroReadRepository>();

        // Script Executor
        services.AddScoped<IScriptExecutor, SqlServerScriptExecutor>();

        return services;
    }
}