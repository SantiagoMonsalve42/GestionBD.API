using GestionBD.Application.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac;

namespace GestionBD.Infraestructure.Services
{
    public sealed class DacpacService : IDacpacService
    {
        private readonly string _defaultOutputPath;

        public DacpacService(IConfiguration configuration)
        {
            _defaultOutputPath = configuration["FileStorage:BasePathDACPAC"]
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
