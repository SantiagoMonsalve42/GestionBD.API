using MediatR;
using GestionBD.Application.Contracts.Artefactos;
using System.Data;
using Dapper;

namespace GestionBD.Application.Artefactos.Queries;

public sealed class GetArtefactoByIdQueryHandler : IRequestHandler<GetArtefactoByIdQuery, ArtefactoResponse?>
{
    private readonly IDbConnection _connection;

    public GetArtefactoByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ArtefactoResponse?> Handle(GetArtefactoByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                a.idArtefacto AS IdArtefacto,
                a.idEntregable AS IdEntregable,
                a.ordenEjecucion AS OrdenEjecucion,
                a.codificacion AS Codificacion,
                a.nombreArtefacto AS NombreArtefacto,
                a.rutaRelativa AS RutaRelativa,
                a.esReverso AS EsReverso,
                e.descripcionEntregable AS DescripcionEntregable
            FROM dbo.tbl_Artefactos a
            INNER JOIN dbo.tbl_Entregables e ON a.idEntregable = e.idEntregable
            WHERE a.idArtefacto = @IdArtefacto;
            """;

        var artefacto = await _connection.QuerySingleOrDefaultAsync<ArtefactoResponse>(sql, new { request.IdArtefacto });
        return artefacto;
    }
}