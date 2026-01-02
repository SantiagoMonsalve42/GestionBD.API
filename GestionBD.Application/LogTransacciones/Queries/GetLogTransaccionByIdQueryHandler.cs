using MediatR;
using GestionBD.Application.Contracts.LogTransacciones;
using System.Data;
using Dapper;

namespace GestionBD.Application.LogTransacciones.Queries;

public sealed class GetLogTransaccionByIdQueryHandler : IRequestHandler<GetLogTransaccionByIdQuery, LogTransaccionResponse?>
{
    private readonly IDbConnection _connection;

    public GetLogTransaccionByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<LogTransaccionResponse?> Handle(GetLogTransaccionByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                idTransaccion AS IdTransaccion,
                nombreTransaccion AS NombreTransaccion,
                estadoTransaccion AS EstadoTransaccion,
                descripcionTransaccion AS DescripcionTransaccion,
                fechaInicio AS FechaInicio,
                respuestaTransaccion AS RespuestaTransaccion,
                fechaFin AS FechaFin,
                usuarioEjecucion AS UsuarioEjecucion
            FROM dbo.tbl_logTransacciones
            WHERE idTransaccion = @IdTransaccion;
            """;

        var logTransaccion = await _connection.QuerySingleOrDefaultAsync<LogTransaccionResponse>(sql, new { request.IdTransaccion });
        return logTransaccion;
    }
}