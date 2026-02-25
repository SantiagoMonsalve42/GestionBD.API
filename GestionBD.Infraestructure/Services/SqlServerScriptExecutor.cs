using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Configuration;
using GestionBD.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace GestionBD.Infrastructure.Services;

public sealed class SqlServerScriptExecutor : IScriptExecutor
{
    private readonly IOptions<DacpacSettings> _configuration;
    private const int CommandTimeout = 300; 

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
        var finalServerName = serverName ?? _configuration.Value.ServerName 
            ?? throw new InvalidOperationException("ServerName no está configurado");
        var finalUsername = username ?? _configuration.Value.Username 
            ?? throw new InvalidOperationException("Username no está configurado");
        var finalPassword = password ?? _configuration.Value.Password 
            ?? throw new InvalidOperationException("Password no está configurado");

        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            finalServerName, 
            databaseName, 
            finalUsername, 
            finalPassword,
            SqlConnectionOptions.ScriptExecutor);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = scriptBatch;
        command.CommandTimeout = CommandTimeout;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
