namespace GestionBD.Application.Configuration;

/// <summary>
/// Configuración de cadenas de conexión
/// </summary>
public sealed class ConnectionStringsSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}