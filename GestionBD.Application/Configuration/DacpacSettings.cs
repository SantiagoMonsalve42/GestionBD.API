namespace GestionBD.Application.Configuration;

/// <summary>
/// Configuración para operaciones DACPAC
/// </summary>
public sealed class DacpacSettings
{
    public string ServerName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}