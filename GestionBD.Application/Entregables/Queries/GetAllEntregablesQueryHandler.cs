using MediatR;
using GestionBD.Application.Contracts.Entregables;
using System.Data;
using Dapper;

namespace GestionBD.Application.Entregables.Queries;

public sealed class GetAllEntregablesQueryHandler : IRequestHandler<GetAllEntregablesQuery, IEnumerable<EntregableResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllEntregablesQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EntregableResponse>> Handle(GetAllEntregablesQuery request, CancellationToken cancellationToken)
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
            ORDER BY ent.idEntregable DESC;
            """;

        var entregables = await _connection.QueryAsync<EntregableResponse>(sql);
        return entregables;
    }
}