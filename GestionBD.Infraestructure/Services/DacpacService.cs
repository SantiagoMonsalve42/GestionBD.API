using GestionBD.Application.Abstractions;
using GestionBD.Application.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;

namespace GestionBD.Infraestructure.Services
{
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

            // Generar nombre único para el DACPAC
            var fileName = $"{Guid.NewGuid()}.dacpac";
            var targetPath = _defaultOutputPath;

            // Asegurar que el directorio existe
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            var fullPath = Path.Combine(targetPath, fileName);

            // Construir la cadena de conexión
            var connectionString = BuildConnectionString(serverName, databaseName, username, password);

            // Crear el servicio DacFx
            var dacServices = new DacServices(connectionString);

            // Opcional: Suscribirse a eventos de progreso
            dacServices.Message += (sender, e) =>
            {
                Console.WriteLine($"DacFx: {e.Message}");
            };

            // Extraer el DACPAC de forma asíncrona
            await Task.Run(() =>
            {
                dacServices.Extract(fullPath, databaseName, "GestionBD", new Version(1, 0, 0, 0));
            }, cancellationToken);

            return fullPath;
        }
        public async Task<string> DeployDacpacToTemporaryDatabaseAsync(string dacpacPath,
                                                                        CancellationToken cancellationToken = default)
        {
           
            if (string.IsNullOrWhiteSpace(dacpacPath))
                throw new ArgumentException("La ruta del DACPAC no puede estar vacía", nameof(dacpacPath));

            if (!File.Exists(dacpacPath))
                throw new FileNotFoundException("El archivo DACPAC no existe", dacpacPath);
            ;
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));

            // Generar nombre único para la BD temporal
            var tempDatabaseName = $"TempDB_{Guid.NewGuid():N}";

            // Crear la base de datos vacía
            await CreateEmptyDatabaseAsync(serverName, tempDatabaseName, username, password, cancellationToken);

            try
            {
                // Desplegar el DACPAC en la BD temporal
                var connectionString = BuildConnectionString(serverName, tempDatabaseName, username, password);
                var dacServices = new DacServices(connectionString);

                dacServices.Message += (sender, e) =>
                {
                    Console.WriteLine($"DacFx Deploy: {e.Message}");
                };

                // Cargar el paquete DACPAC
                using var dacPackage = DacPackage.Load(dacpacPath);

                // Configurar opciones de despliegue
                var deployOptions = new DacDeployOptions
                {
                    BlockOnPossibleDataLoss = false,
                    CreateNewDatabase = false, // Ya creamos la BD
                    IgnorePermissions = true,
                    IgnoreRoleMembership = true,
                    IgnoreUserSettingsObjects = true,
                    IgnoreLoginSids = true,
                    GenerateSmartDefaults = true,
                    ScriptDatabaseOptions = false
                };

                // Ejecutar el despliegue
                await Task.Run(() =>
                {
                    dacServices.Deploy(dacPackage, tempDatabaseName, upgradeExisting: true, options: deployOptions);
                }, cancellationToken);

                return tempDatabaseName;
            }
            catch
            {
                // Si falla el despliegue, eliminar la BD temporal
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

        public async Task DropTemporaryDatabaseAsync( string databaseName,
                                                      CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("El nombre de la base de datos no puede estar vacío", nameof(databaseName));

            // Verificar que es una BD temporal (seguridad)
            if (!databaseName.StartsWith("TempDB_", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Solo se pueden eliminar bases de datos temporales (TempDB_*)");

            var connectionString = BuildConnectionString(serverName, "master", username, password);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            // Cerrar todas las conexiones activas a la BD
            var killConnectionsCommand = connection.CreateCommand();
            killConnectionsCommand.CommandText = $@"
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            ";
            await killConnectionsCommand.ExecuteNonQueryAsync(cancellationToken);

            // Eliminar la base de datos
            var dropCommand = connection.CreateCommand();
            dropCommand.CommandText = $"DROP DATABASE [{databaseName}];";
            await dropCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task CreateEmptyDatabaseAsync(string serverName,
                                                            string databaseName,
                                                            string? username,
                                                            string? password,
                                                            CancellationToken cancellationToken)
        {
            var connectionString = BuildConnectionString(serverName, "master", username, password);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE [{databaseName}];";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        private static string BuildConnectionString(string serverName,
                                                     string databaseName,
                                                     string? username,
                                                     string? password)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                InitialCatalog = databaseName,
                TrustServerCertificate = true,
                ConnectTimeout = 30
            };

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
    }
}
