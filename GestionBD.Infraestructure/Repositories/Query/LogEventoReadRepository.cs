using System.Data;
using Dapper;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.LogEventos;

namespace GestionBD.Infraestructure.Repositories.Query;

public sealed class LogEventoReadRepository : ILogEventoReadRepository
{
    private readonly IDbConnection _connection;

    public LogEventoReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<LogEventoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                le.idEvento AS IdEvento,
                le.idTransaccion AS IdTransaccion,
                le.fechaEjecucion AS FechaEjecucion,
                le.descripcion AS Descripcion,
                le.estadoEvento AS EstadoEvento,
                lt.nombreTransaccion AS NombreTransaccion
            FROM dbo.tbl_logEventos le
            INNER JOIN dbo.tbl_logTransacciones lt ON le.idTransaccion = lt.idTransaccion
            ORDER BY le.fechaEjecucion DESC;
            """;

        return await _connection.QueryAsync<LogEventoResponse>(sql);
    }

    public async Task<LogEventoResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                le.idEvento AS IdEvento,
                le.idTransaccion AS IdTransaccion,
                le.fechaEjecucion AS FechaEjecucion,
                le.descripcion AS Descripcion,
                le.estadoEvento AS EstadoEvento,
                lt.nombreTransaccion AS NombreTransaccion
            FROM dbo.tbl_logEventos le
            INNER JOIN dbo.tbl_logTransacciones lt ON le.idTransaccion = lt.idTransaccion
            WHERE le.idEvento = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<LogEventoResponse>(sql, new { Id = id });
    }
}