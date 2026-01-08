namespace GestionBD.Application.Configuration;

/// <summary>
/// Configuración para almacenamiento de archivos
/// </summary>
public sealed class FileStorageSettings
{
    public string BasePath { get; set; } = string.Empty;
    public string BasePathDACPAC { get; set; } = string.Empty;
}