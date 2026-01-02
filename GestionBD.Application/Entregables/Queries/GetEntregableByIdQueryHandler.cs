using MediatR;
using GestionBD.Application.Contracts.Entregables;
using System.Data;
using Dapper;

namespace GestionBD.Application.Entregables.Queries;

public sealed class GetEntregableByIdQueryHandler : IRequestHandler<GetEntregableByIdQuery, EntregableResponse?>
{
    private readonly IDbConnection _connection;

    public GetEntregableByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<EntregableResponse?> Handle(GetEntregableByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                ent.idEntregable AS IdEntregable,
                ent.rutaEntregable AS RutaEntregable,
                ent.descripcionEntregable AS DescripcionEntregable,
                ent.idEjecucion AS IdEjecucion,
                ej.descripcion AS DescripcionEjecucion
            FROM dbo.tbl_Entregables ent
            INNER JOIN dbo.tbl_Ejecuciones ej ON ent.idEjecucion = ej.idEjecucion
            WHERE ent.idEntregable = @IdEntregable;
            """;

        var entregable = await _connection.QuerySingleOrDefaultAsync<EntregableResponse>(sql, new { request.IdEntregable });
        return entregable;
    }
}