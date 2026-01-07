using System.Data;
using Dapper;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Artefactos;

namespace GestionBD.Infraestructure.Repositories.Query;

public sealed class ArtefactoReadRepository : IArtefactoReadRepository
{
    private readonly IDbConnection _connection;

    public ArtefactoReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<ArtefactoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
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

        return await _connection.QueryAsync<ArtefactoResponse>(sql);
    }

    public async Task<IEnumerable<ArtefactoResponse>> GetByEntregableIdAsync(decimal id, CancellationToken cancellationToken = default)
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
            WHERE a.idEntregable = @Id
            ORDER BY a.ordenEjecucion;
            """;

        return await _connection.QueryAsync<ArtefactoResponse>(sql, new {Id = id});
    }

    public async Task<ArtefactoResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
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
            WHERE a.idArtefacto = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<ArtefactoResponse>(sql, new { Id = id });
    }
}