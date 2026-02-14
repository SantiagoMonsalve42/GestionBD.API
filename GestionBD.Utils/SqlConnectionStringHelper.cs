using Microsoft.Data.SqlClient;

namespace GestionBD.Utils;

/// <summary>
/// Utilidad para construir connection strings de SQL Server de forma consistente
/// </summary>
public static class SqlConnectionStringHelper
{
    /// <summary>
    /// Construye un connection string de SQL Server con configuración estándar
    /// </summary>
    /// <param name="serverName">Nombre del servidor</param>
    /// <param name="databaseName">Nombre de la base de datos</param>
    /// <param name="username">Usuario (opcional para Windows Auth)</param>
    /// <param name="password">Contraseña (opcional para Windows Auth)</param>
    /// <param name="options">Opciones adicionales de conexión</param>
    /// <returns>Connection string configurado</returns>
    public static string BuildConnectionString(
        string serverName, 
        string databaseName, 
        string? username = null, 
        string? password = null,
        SqlConnectionOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));
        
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("El nombre de la base de datos no puede estar vacío", nameof(databaseName));

        var effectiveOptions = options ?? SqlConnectionOptions.Default;

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = serverName,
            InitialCatalog = databaseName,
            TrustServerCertificate = effectiveOptions.TrustServerCertificate,
            ConnectTimeout = effectiveOptions.ConnectTimeout,
            Encrypt = effectiveOptions.Encrypt
        };

        // Configurar CommandTimeout si está especificado
        if (effectiveOptions.CommandTimeout.HasValue)
        {
            builder.CommandTimeout = effectiveOptions.CommandTimeout.Value;
        }

        // Configurar autenticación
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            builder.UserID = username;
            builder.Password = password;
            builder.IntegratedSecurity = false;
        }
        else
        {
            builder.IntegratedSecurity = true;
        }

        return builder.ConnectionString;
    }

    /// <summary>
    /// Construye un connection string usando autenticación de Windows
    /// </summary>
    /// <param name="serverName">Nombre del servidor</param>
    /// <param name="databaseName">Nombre de la base de datos</param>
    /// <param name="options">Opciones adicionales de conexión</param>
    /// <returns>Connection string con autenticación de Windows</returns>
    public static string BuildWindowsAuthConnectionString(
        string serverName, 
        string databaseName, 
        SqlConnectionOptions? options = null)
    {
        return BuildConnectionString(serverName, databaseName, null, null, options);
    }

    /// <summary>
    /// Construye un connection string usando autenticación SQL Server
    /// </summary>
    /// <param name="serverName">Nombre del servidor</param>
    /// <param name="databaseName">Nombre de la base de datos</param>
    /// <param name="username">Usuario SQL Server</param>
    /// <param name="password">Contraseña SQL Server</param>
    /// <param name="options">Opciones adicionales de conexión</param>
    /// <returns>Connection string con autenticación SQL Server</returns>
    public static string BuildSqlAuthConnectionString(
        string serverName, 
        string databaseName, 
        string username, 
        string password, 
        SqlConnectionOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El usuario no puede estar vacío para autenticación SQL", nameof(username));
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("La contraseña no puede estar vacía para autenticación SQL", nameof(password));

        return BuildConnectionString(serverName, databaseName, username, password, options);
    }
}

/// <summary>
/// Opciones de configuración para connection strings de SQL Server
/// </summary>
public sealed record SqlConnectionOptions
{
    /// <summary>
    /// Configuración por defecto
    /// </summary>
    public static readonly SqlConnectionOptions Default = new();

    /// <summary>
    /// Configuración para DatabaseService (con CommandTimeout)
    /// </summary>
    public static readonly SqlConnectionOptions DatabaseService = new()
    {
        CommandTimeout = 30
    };

    /// <summary>
    /// Configuración para SqlServerScriptExecutor (sin encriptación)
    /// </summary>
    public static readonly SqlConnectionOptions ScriptExecutor = new()
    {
        Encrypt = false
    };

    /// <summary>
    /// Configuración para operaciones de larga duración
    /// </summary>
    public static readonly SqlConnectionOptions LongRunning = new()
    {
        ConnectTimeout = 60,
        CommandTimeout = 300 // 5 minutos
    };

    public bool TrustServerCertificate { get; init; } = true;
    public int ConnectTimeout { get; init; } = 30;
    public bool Encrypt { get; init; } = true;
    public int? CommandTimeout { get; init; } = null;
}