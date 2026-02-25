using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Configuration;

namespace GestionBD.Infrastructure.ExternalServices.Vault;

/// <summary>
/// Cargador de configuraciones desde Vault al inicio de la aplicación
/// </summary>
public sealed class VaultConfigurationLoader
{
    private readonly IVaultConfigurationProvider _vaultProvider;

    public VaultConfigurationLoader(IVaultConfigurationProvider vaultProvider)
    {
        _vaultProvider = vaultProvider;
    }

    public async Task<DacpacSettings> LoadDacpacSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _vaultProvider.GetConfigurationAsync<DacpacSettings>(
            "gestionbd/DacpacSettings",
            cancellationToken);
    }

    public async Task<FileStorageSettings> LoadFileStorageSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _vaultProvider.GetConfigurationAsync<FileStorageSettings>(
            "gestionbd/FileStorage",
            cancellationToken);
    }

    public async Task<ConnectionStringsSettings> LoadConnectionStringsAsync(CancellationToken cancellationToken = default)
    {
        return await _vaultProvider.GetConfigurationAsync<ConnectionStringsSettings>(
            "gestionbd/ConnectionStrings",
            cancellationToken);
    }

    public async Task<OpenIASettings> LoadOpenIASettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _vaultProvider.GetConfigurationAsync<OpenIASettings>(
            "gestionbd/OpenIASettings",
            cancellationToken);
    }

    public async Task<KeycloakSettings> LoadKeycloakSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _vaultProvider.GetConfigurationAsync<KeycloakSettings>(
            "gestionbd/KeycloakSettings",
            cancellationToken);
    }
}