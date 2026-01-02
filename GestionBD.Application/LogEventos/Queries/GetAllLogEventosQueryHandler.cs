using MediatR;
using GestionBD.Application.Contracts.LogEventos;
using System.Data;
using Dapper;

namespace GestionBD.Application.LogEventos.Queries;

public sealed class GetAllLogEventosQueryHandler : IRequestHandler<GetAllLogEventosQuery, IEnumerable<LogEventoResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllLogEventosQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<LogEventoResponse>> Handle(GetAllLogEventosQuery request, CancellationToken cancellationToken)
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

        var logEventos = await _connection.QueryAsync<LogEventoResponse>(sql);
        return logEventos;
    }
}