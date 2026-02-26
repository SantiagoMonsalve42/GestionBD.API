using GestionBD.Application.Abstractions.Config;
using System.Text.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace GestionBD.Infrastructure.ExternalServices.Vault;

/// <summary>
/// Implementación del proveedor de configuración desde HashiCorp Vault
/// </summary>
public sealed class HashiCorpVaultConfigurationProvider : IVaultConfigurationProvider
{
    private readonly IVaultClient _vaultClient;

    public HashiCorpVaultConfigurationProvider(string vaultUri, string token)
    {
        if (string.IsNullOrWhiteSpace(vaultUri))
            throw new ArgumentException("VaultUri no puede estar vacío", nameof(vaultUri));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token no puede estar vacío", nameof(token));

        var authMethod = new TokenAuthMethodInfo(token);
        var vaultClientSettings = new VaultClientSettings(vaultUri, authMethod);
        _vaultClient = new VaultClient(vaultClientSettings);
    }

    public async Task<T> GetConfigurationAsync<T>(string path, CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var secrets = await GetSecretsAsync(path, cancellationToken);

        var json = JsonSerializer.Serialize(secrets);
        var result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? new T();
    }

    public async Task<IDictionary<string, object>> GetSecretsAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("La ruta no puede estar vacía", nameof(path));

        try
        {
            Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2
                .ReadSecretAsync(path: path, mountPoint: "secret");

            if (secret?.Data?.Data == null)
            {
                throw new InvalidOperationException($"No se encontraron datos en la ruta '{path}'");
            }

            return secret.Data.Data;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error al obtener configuración de Vault. Ruta: '{path}'", ex);
        }
    }

    public async Task SetSecretsAsync(string path, object secrets, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("La ruta no puede estar vacía", nameof(path));

        if (secrets == null)
            throw new ArgumentNullException(nameof(secrets));

        var data = ToDictionary(secrets);

        if (data.Count == 0)
            throw new ArgumentException("El objeto de secretos no puede estar vacío", nameof(secrets));

        try
        {
            await _vaultClient.V1.Secrets.KeyValue.V2
                .WriteSecretAsync(path: path, data: data, mountPoint: "secret");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error al guardar configuración en Vault. Ruta: '{path}'", ex);
        }
    }

    private static Dictionary<string, object> ToDictionary(object secrets)
    {
        var json = JsonSerializer.Serialize(secrets);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json)
               ?? new Dictionary<string, object>();
    }
}