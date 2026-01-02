using MediatR;
using GestionBD.Application.Contracts.Artefactos;
using System.Data;
using Dapper;

namespace GestionBD.Application.Artefactos.Queries;

public sealed class GetAllArtefactosQueryHandler : IRequestHandler<GetAllArtefactosQuery, IEnumerable<ArtefactoResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllArtefactosQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<ArtefactoResponse>> Handle(GetAllArtefactosQuery request, CancellationToken cancellationToken)
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
            ORDER BY a.ordenEjecucion;
            """;

        var artefactos = await _connection.QueryAsync<ArtefactoResponse>(sql);
        return artefactos;
    }
}