using GestionBD.Application.Abstractions.Config;
using GestionBD.Domain.Services;
using GestionBD.Infraestructure.ExternalServices.OpenAI;
using GestionBD.Infraestructure.ExternalServices.Vault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionBD.Infraestructure.ExternalServices;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddExternalServicesAsync(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Configurar el proveedor de Vault
        var vaultUri = configuration["Vault:Uri"]
            ?? throw new InvalidOperationException("Vault:Uri no está configurado");

        var vaultToken = configuration["Vault:Token"]
            ?? throw new InvalidOperationException("Vault:Token no está configurado");

        // 2. Registrar el proveedor de Vault
        var vaultProvider = new HashiCorpVaultConfigurationProvider(vaultUri, vaultToken);
        services.AddSingleton<IVaultConfigurationProvider>(vaultProvider);

        // 3. Cargar configuraciones desde Vault
        var loader = new VaultConfigurationLoader(vaultProvider);

        var dacpacSettings = await loader.LoadDacpacSettingsAsync();
        var fileStorageSettings = await loader.LoadFileStorageSettingsAsync();
        var connectionStringsSettings = await loader.LoadConnectionStringsAsync();
        var openIASettings = await loader.LoadOpenIASettingsAsync();
        var keycloakSettings = await loader.LoadKeycloakSettingsAsync();

        // 4. Registrar configuraciones con IOptions
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(dacpacSettings));
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(fileStorageSettings));
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(connectionStringsSettings));
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(openIASettings));
        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(keycloakSettings));

        // 5. Registrar HttpClient y servicio de OpenAI
        services.AddHttpClient<ISqlValidationService, OpenAISqlValidationService>();
        services.AddHttpClient<IRollbackGenerationService, OpenAIRollbackGenerationService>();

        return services;
    }
}