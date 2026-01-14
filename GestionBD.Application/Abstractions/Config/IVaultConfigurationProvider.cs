namespace GestionBD.Application.Abstractions.Config;

/// <summary>
/// Proveedor de configuración desde HashiCorp Vault
/// </summary>
public interface IVaultConfigurationProvider
{
    /// <summary>
    /// Obtiene configuración desde una ruta específica y la deserializa al tipo T
    /// </summary>
    Task<T> GetConfigurationAsync<T>(string path, CancellationToken cancellationToken = default) where T : class, new();
    
    /// <summary>
    /// Obtiene todos los datos de una ruta como diccionario
    /// </summary>
    Task<IDictionary<string, object>> GetSecretsAsync(string path, CancellationToken cancellationToken = default);
}