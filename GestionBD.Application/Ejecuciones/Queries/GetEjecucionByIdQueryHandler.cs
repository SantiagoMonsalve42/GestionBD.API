using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;
using System.Data;
using Dapper;

namespace GestionBD.Application.Ejecuciones.Queries;

public sealed class GetEjecucionByIdQueryHandler : IRequestHandler<GetEjecucionByIdQuery, EjecucionResponse?>
{
    private readonly IDbConnection _connection;

    public GetEjecucionByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<EjecucionResponse?> Handle(GetEjecucionByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                e.idEjecucion AS IdEjecucion,
                e.idInstancia AS IdInstancia,
                e.horaInicioEjecucion AS HoraInicioEjecucion,
                e.horaFinEjecucion AS HoraFinEjecucion,
                e.descripcion AS Descripcion,
                i.instancia AS NombreInstancia
            FROM dbo.tbl_Ejecuciones e
            INNER JOIN dbo.tbl_Instancias i ON e.idInstancia = i.idInstancia
            WHERE e.idEjecucion = @IdEjecucion;
            """;

        var ejecucion = await _connection.QuerySingleOrDefaultAsync<EjecucionResponse>(sql, new { request.IdEjecucion });
        return ejecucion;
    }
}