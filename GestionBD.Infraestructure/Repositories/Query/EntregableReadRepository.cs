using System.Data;
using Dapper;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Infraestructure.Repositories.Query;

public sealed class EntregableReadRepository : IEntregableReadRepository
{
    private readonly IDbConnection _connection;

    public EntregableReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EntregableResponse>> GetAllAsync(CancellationToken cancellationToken = default)
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

        return await _connection.QueryAsync<EntregableResponse>(sql);
    }

    public async Task<EntregableResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
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
            WHERE ent.idEntregable = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<EntregableResponse>(sql, new { Id = id });
    }
}