using System.Data;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Utils;
using Microsoft.Data.SqlClient;

namespace GestionBD.Infrastructure.Services;

public sealed class DatabaseService : IDatabaseService
{
    

    public async Task<string> getObjectDefinition(
        string serverName,
        string databaseName,
        string user,
        string password,
        string objectName)
    {
        if (string.IsNullOrWhiteSpace(serverName))
            throw new ArgumentException("El nombre del servidor no puede estar vacío", nameof(serverName));

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new ArgumentException("El nombre de la base de datos no puede estar vacío", nameof(databaseName));

        if (string.IsNullOrWhiteSpace(objectName))
            throw new ArgumentException("El nombre del objeto no puede estar vacío", nameof(objectName));

        var connectionString = SqlConnectionStringHelper.BuildConnectionString(
            serverName, 
            databaseName, 
            user, 
            password, 
            SqlConnectionOptions.DatabaseService);

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var (schemaName, parsedObjectName) = ParseObjectName(objectName);

            var definition = await GetObjectDefinitionAsync(connection, schemaName, parsedObjectName);

            return definition;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Error al conectar con SQL Server: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al obtener la definición del objeto '{objectName}': {ex.Message}", ex);
        }
    }

    private static async Task<string> GetObjectDefinitionAsync(
        SqlConnection connection,
        string schemaName,
        string objectName)
    {
        const string sql = """
            SELECT 
                o.type_desc AS ObjectType,
                OBJECT_DEFINITION(o.object_id) AS Definition
            FROM sys.objects o
            INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
            WHERE s.name = @SchemaName 
                AND o.name = @ObjectName
                AND o.type IN ('P', 'FN', 'IF', 'TF', 'V', 'TR')  -- Solo objetos compatibles con OBJECT_DEFINITION
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@SchemaName", SqlDbType.NVarChar, 128).Value = schemaName;
        command.Parameters.Add("@ObjectName", SqlDbType.NVarChar, 128).Value = objectName;
        command.CommandTimeout = SqlConnectionOptions.DatabaseService.ConnectTimeout;

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var objectType = reader.GetString("ObjectType");
            var definition = reader.IsDBNull("Definition") ? null : reader.GetString("Definition");

            if (string.IsNullOrWhiteSpace(definition))
                return $"-- El objeto [{schemaName}].[{objectName}] (Tipo: {objectType}) no tiene definición disponible o está encriptado";

            return $"-- Objeto: [{schemaName}].[{objectName}] (Tipo: {objectType})\n{definition}";
        }

        return string.Empty;
    }

    private static (string schema, string objectName) ParseObjectName(string fullObjectName)
    {
        var cleanName = fullObjectName.Trim('[', ']');

        var parts = cleanName.Split('.');
        return parts.Length switch
        {
            1 => ("dbo", parts[0]),
            2 => (parts[0], parts[1]),
            3 => (parts[1], parts[2]),
            _ => throw new ArgumentException($"Formato de nombre de objeto inválido: {fullObjectName}")
        };
    }
}
