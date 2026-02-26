using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.DTO.Vault;

namespace GestionBD.Application.Instancias.Helpers;

public static class VaultPathHelper
{
    public static async Task<string> CreateVaultPathAsync(
        IVaultConfigurationProvider vaultConfigurationProvider,
        string user,
        string pass,
        decimal idInstancia,
        CancellationToken cancellationToken)
    {
        if (vaultConfigurationProvider == null)
            throw new ArgumentNullException(nameof(vaultConfigurationProvider));

        var vaultPath = $"secret/{idInstancia.ToString()}/bd";
        var credentialVaultDTO = new CredentialVaultDTO(user, pass);
        await vaultConfigurationProvider.SetSecretsAsync(vaultPath, credentialVaultDTO, cancellationToken);
        return vaultPath;
    }
}