using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;
using System.Data;
using Dapper;

namespace GestionBD.Application.Ejecuciones.Queries;

public sealed class GetAllEjecucionesQueryHandler : IRequestHandler<GetAllEjecucionesQuery, IEnumerable<EjecucionResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllEjecucionesQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EjecucionResponse>> Handle(GetAllEjecucionesQuery request, CancellationToken cancellationToken)
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
            ORDER BY e.horaInicioEjecucion DESC;
            """;

        var ejecuciones = await _connection.QueryAsync<EjecucionResponse>(sql);
        return ejecuciones;
    }
}