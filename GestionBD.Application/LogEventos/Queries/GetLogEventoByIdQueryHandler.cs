using MediatR;
using GestionBD.Application.Contracts.LogEventos;
using System.Data;
using Dapper;

namespace GestionBD.Application.LogEventos.Queries;

public sealed class GetLogEventoByIdQueryHandler : IRequestHandler<GetLogEventoByIdQuery, LogEventoResponse?>
{
    private readonly IDbConnection _connection;

    public GetLogEventoByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<LogEventoResponse?> Handle(GetLogEventoByIdQuery request, CancellationToken cancellationToken)
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
            WHERE le.idEvento = @IdEvento;
            """;

        var logEvento = await _connection.QuerySingleOrDefaultAsync<LogEventoResponse>(sql, new { request.IdEvento });
        return logEvento;
    }
}