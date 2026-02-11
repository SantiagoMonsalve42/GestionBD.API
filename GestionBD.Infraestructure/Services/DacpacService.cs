using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Configuration;
using GestionBD.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;

namespace GestionBD.Infraestructure.Services;

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

        var killConnectionsCommand = connection.CreateCommand();
        killConnectionsCommand.CommandText = $@"
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            ";
        await killConnectionsCommand.ExecuteNonQueryAsync(cancellationToken);

        var dropCommand = connection.CreateCommand();
        dropCommand.CommandText = $"DROP DATABASE [{databaseName}];";
        await dropCommand.ExecuteNonQueryAsync(cancellationToken);
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
