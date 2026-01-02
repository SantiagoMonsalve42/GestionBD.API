using MediatR;
using GestionBD.Application.Contracts.LogTransacciones;
using System.Data;
using Dapper;

namespace GestionBD.Application.LogTransacciones.Queries;

public sealed class GetAllLogTransaccionesQueryHandler : IRequestHandler<GetAllLogTransaccionesQuery, IEnumerable<LogTransaccionResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllLogTransaccionesQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<LogTransaccionResponse>> Handle(GetAllLogTransaccionesQuery request, CancellationToken cancellationToken)
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
            ORDER BY fechaInicio DESC;
            """;

        var logTransacciones = await _connection.QueryAsync<LogTransaccionResponse>(sql);
        return logTransacciones;
    }
}