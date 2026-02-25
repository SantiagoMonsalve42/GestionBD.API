using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Configuration;
using GestionBD.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;

namespace GestionBD.Infrastructure.Services;

public sealed class DacpacService : IDacpacService
{
    private readonly string _defaultOutputPath;
    private readonly IOptions<DacpacSettings> _configuration;
    private readonly IOptions<FileStorageSettings> _configurationPath;
    private string serverName => _configuration.Value.ServerName ?? throw new InvalidOperationException("Database:ServerName no está configurado");
    private string username => _configuration.Value.Username ?? throw new InvalidOperationException("Database:Username no está configurado");
    private string password => _configuration.Value.Password ?? throw new InvalidOperationException("Database:Password no está configurado");

    public DacpacService(IOptions<DacpacSettings> configuration, IOptions<FileStorageSettings> configurationPath)
    {
        _configuration = configuration;
        _configurationPath = configurationPath;
        _defaultOutputPath = _configurationPath.Value.BasePathDACPAC
            ?? throw new InvalidOperationException("FileStorage:BasePathDACPAC no está configurado");

        if (!Directory.Exists(_defaultOutputPath))
        {
            Directory.CreateDirectory(_defaultOutputPath);
        }
    }

    public async Task<string> ExtractDacpacAsync(string serverName,
                                                  string databaseName,
                                                  string? username = null,
                                                  string? password = null,
                                                  CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("El nombre de la base de datos no puede estar vacío", nameof(databaseName));

        var fileName = $"{Guid.NewGuid()}.dacpac";
        var targetPath = _defaultOutputPath;

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        var fullPath = Path.Combine(targetPath, fileName);

        // ✅ Usar la nueva clase Utils
        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            serverName, 
            databaseName, 
            username, 
            password);

        var dacServices = new DacServices(connectionString);

        dacServices.Message += (sender, e) =>
        {
            Console.WriteLine($"DacFx: {e.Message}");
        };

        await Task.Run(() =>
        {
            dacServices.Extract(fullPath, databaseName, "GestionBD", new Version(1, 0, 0, 0));
        }, cancellationToken);

        return fullPath;
    }

    public async Task<string> DeployDacpacToTemporaryDatabaseAsync(string dacpacPath,
                                                                   string? bdName = null,
                                                                    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dacpacPath))
            throw new ArgumentException("La ruta del DACPAC no puede estar vacía", nameof(dacpacPath));

        if (!File.Exists(dacpacPath))
            throw new FileNotFoundException("El archivo DACPAC no existe", dacpacPath);

        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));

        var tempDatabaseName = bdName;
        if (bdName == null)
        {
            tempDatabaseName = $"TempDB_{Guid.NewGuid():N}";
            await CreateEmptyDatabaseAsync(serverName, tempDatabaseName, username, password, cancellationToken);
        }
        
        try
        {
            // ✅ Usar la nueva clase Utils
            var connectionString = SqlConnectionStringHelper.BuildConnectionString(
                serverName, 
                tempDatabaseName, 
                username, 
                password);

            var dacServices = new DacServices(connectionString);

            dacServices.Message += (sender, e) =>
            {
                Console.WriteLine($"DacFx Deploy: {e.Message}");
            };

            using var dacPackage = DacPackage.Load(dacpacPath);

            var deployOptions = new DacDeployOptions
            {
                BlockOnPossibleDataLoss = false,
                CreateNewDatabase = false,
                IgnorePermissions = true,
                IgnoreRoleMembership = true,
                IgnoreUserSettingsObjects = true,
                IgnoreLoginSids = true,
                GenerateSmartDefaults = true,
                ScriptDatabaseOptions = false,
                DropObjectsNotInSource = true
            };

            await Task.Run(() =>
            {
                dacServices.Deploy(dacPackage, tempDatabaseName, upgradeExisting: true, options: deployOptions);
            }, cancellationToken);

            return tempDatabaseName;
        }
        catch
        {
            try
            {
                await DropTemporaryDatabaseAsync(tempDatabaseName, cancellationToken);
            }
            catch
            {
                // Ignorar errores al limpiar
            }
            throw;
        }
    }

    public async Task DropTemporaryDatabaseAsync(string databaseName,
                                              CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("El nombre de la base de datos no puede estar vacío", nameof(databaseName));

        if (!databaseName.StartsWith("TempDB_", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Solo se pueden eliminar bases de datos temporales (TempDB_*)");

        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            serverName,
            "master",
            username,
            password);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        // 1. Verificar si la base de datos existe
        var checkExistsCommand = connection.CreateCommand();
        checkExistsCommand.CommandText = """
        SELECT COUNT(1) 
        FROM sys.databases 
        WHERE name = @DatabaseName
        """;
        checkExistsCommand.Parameters.AddWithValue("@DatabaseName", databaseName);

        var exists = (int)await checkExistsCommand.ExecuteScalarAsync(cancellationToken) > 0;

        if (!exists)
        {
            Console.WriteLine($"La base de datos '{databaseName}' no existe.");
            return;
        }

        try
        {
            // 2. Script SQL dinámico para cerrar conexiones
            var killConnectionsCommand = connection.CreateCommand();
            killConnectionsCommand.CommandText = $"""
            IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @DatabaseName)
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            END
            """;
            killConnectionsCommand.Parameters.AddWithValue("@DatabaseName", databaseName);
            await killConnectionsCommand.ExecuteNonQueryAsync(cancellationToken);

            // 3. Script SQL dinámico para eliminar la base de datos
            var dropCommand = connection.CreateCommand();
            dropCommand.CommandText = $"""
            IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @DatabaseName)
            BEGIN
                DROP DATABASE [{databaseName}];
            END
            """;
            dropCommand.Parameters.AddWithValue("@DatabaseName", databaseName);
            await dropCommand.ExecuteNonQueryAsync(cancellationToken);

            Console.WriteLine($"Base de datos temporal '{databaseName}' eliminada exitosamente.");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error SQL al eliminar la base de datos '{databaseName}': {ex.Message}");
            throw;
        }
    }
    private async Task CreateEmptyDatabaseAsync(string serverName,
                                                        string databaseName,
                                                        string? username,
                                                        string? password,
                                                        CancellationToken cancellationToken)
    {
        // ✅ Usar la nueva clase Utils
        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            serverName, 
            "master", 
            username, 
            password);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE [{databaseName}];";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
