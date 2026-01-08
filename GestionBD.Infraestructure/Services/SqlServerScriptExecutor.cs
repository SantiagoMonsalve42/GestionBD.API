using GestionBD.Application.Abstractions;
using GestionBD.Application.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace GestionBD.Infraestructure.Services
{
    public sealed class SqlServerScriptExecutor : IScriptExecutor
    {
        private readonly IOptions<DacpacSettings> _configuration;
        private string ServerName => _configuration.Value.ServerName ?? throw new InvalidOperationException("Database:ServerName no está configurado");
        private string Username => _configuration.Value.Username ?? throw new InvalidOperationException("Database:Username no está configurado");
        private string Password => _configuration.Value.Password ?? throw new InvalidOperationException("Database:Password no está configurado");
        private const int CommandTimeout = 300; // 5 minutos

        public SqlServerScriptExecutor(IOptions<DacpacSettings> configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task ExecuteAsync(string databaseName, string scriptBatch, CancellationToken cancellationToken = default)
        {
            var connectionString = BuildConnectionString(databaseName);

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = scriptBatch;
            command.CommandTimeout = CommandTimeout;

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private string BuildConnectionString(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerName,
                InitialCatalog = databaseName,
                TrustServerCertificate = true,
                ConnectTimeout = 30,
                Encrypt = false
            };

            builder.UserID = Username;
            builder.Password = Password;
            builder.IntegratedSecurity = false;

            return builder.ConnectionString;
        }
    }
}
