using GestionBD.Application.Abstractions.Config;
using System.Text.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace GestionBD.Infraestructure.ExternalServices.Vault;

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
        
        // Convertir el diccionario a JSON y luego deserializar al tipo T
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
            // Leer secreto desde Vault (KV v2)
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
}