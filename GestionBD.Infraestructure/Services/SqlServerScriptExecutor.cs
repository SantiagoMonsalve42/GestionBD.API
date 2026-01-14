using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace GestionBD.Infraestructure.Services
{
    public sealed class SqlServerScriptExecutor : IScriptExecutor
    {
        private readonly IOptions<DacpacSettings> _configuration;
        private const int CommandTimeout = 300; // 5 minutos

        public SqlServerScriptExecutor(IOptions<DacpacSettings> configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task ExecuteAsync(
            string databaseName, 
            string scriptBatch, 
            string? serverName = null,
            string? username = null,
            string? password = null,
            CancellationToken cancellationToken = default)
        {
            // Usa los valores dinámicos si se proporcionan, sino usa la configuración
            var finalServerName = serverName ?? _configuration.Value.ServerName 
                ?? throw new InvalidOperationException("ServerName no está configurado");
            var finalUsername = username ?? _configuration.Value.Username 
                ?? throw new InvalidOperationException("Username no está configurado");
            var finalPassword = password ?? _configuration.Value.Password 
                ?? throw new InvalidOperationException("Password no está configurado");

            var connectionString = BuildConnectionString(databaseName, finalServerName, finalUsername, finalPassword);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = scriptBatch;
            command.CommandTimeout = CommandTimeout;

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private string BuildConnectionString(string databaseName, string serverName, string username, string password)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                InitialCatalog = databaseName,
                UserID = username,
                Password = password,
                IntegratedSecurity = false,
                TrustServerCertificate = true,
                ConnectTimeout = 30,
                Encrypt = false
            };

            return builder.ConnectionString;
        }
    }
}
