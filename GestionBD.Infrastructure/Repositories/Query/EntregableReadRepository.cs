using Dapper;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Entregables;
using System.Data;

namespace GestionBD.Infrastructure.Repositories.Query;

public sealed class EntregableReadRepository : IEntregableReadRepository
{
    private readonly IDbConnection _connection;

    public EntregableReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EntregableResponseEstado>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
              SELECT 
                ent.idEntregable AS IdEntregable,
                ent.rutaEntregable AS RutaEntregable,
                ent.descripcionEntregable AS DescripcionEntregable,
                ent.idEjecucion AS IdEjecucion, 
            	ent.numeroEntrega as NumeroEntrega,
            	ent.rutaDACPAC as RutaDACPAC,
            	ent.temporalBD as TemporalBD,
                est.descripcionEstado as EstadoEntrega,
            	est.idEstado as EstadoEntregaId,
                ent.rutaResultado as RutaResultado,
                ent.rutaRollbackGenerado as RutaResultadoRollback
            FROM dbo.tbl_Entregables ent
            JOIN dbo.tbl_EstadoEntrega est
            on ent.idEstado = est.idEstado
            ORDER BY ent.idEntregable DESC;
            """;

        return await _connection.QueryAsync<EntregableResponseEstado>(sql);
    }

    public async Task<IEnumerable<EntregableResponseEstado>> GetAllByIdEjecucionAsync(decimal idEjecucion, CancellationToken cancellationToken = default)
    {
        const string sql = """
                SELECT 
                ent.idEntregable AS IdEntregable,
                ent.rutaEntregable AS RutaEntregable,
                ent.descripcionEntregable AS DescripcionEntregable,
                ent.idEjecucion AS IdEjecucion, 
               	ent.numeroEntrega as NumeroEntrega,
               	ent.rutaDACPAC as RutaDACPAC,
               	ent.temporalBD as TemporalBD,
            	est.descripcionEstado as EstadoEntrega,
            	est.idEstado as EstadoEntregaId,
                ent.rutaResultado as RutaResultado,
                ent.rutaRollbackGenerado as RutaResultadoRollback
            FROM dbo.tbl_Entregables ent
            JOIN dbo.tbl_EstadoEntrega est
            on ent.idEstado = est.idEstado
            where ent.idEjecucion = @Id
            ORDER BY ent.idEntregable ;
            """;

        return await _connection.QueryAsync<EntregableResponseEstado>(sql, new { Id = idEjecucion });
    }

    public async Task<IEnumerable<EntregableRevisionResponse>> GetAllRevisionesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
              SELECT  en.idEntregable as IdEntregable,
            		en.rutaEntregable as RutaEntregable,
            		ej.nombreRequerimiento as NombreRequerimiento,
            		ej.descripcion as DescripcionEntregable,
            		en.numeroEntrega as NumeroEntrega
              FROM [dbo].[tbl_Entregables] en
              JOIN [dbo].[tbl_Ejecuciones] ej
              ON en.idEjecucion = ej.idEjecucion
              where idEstado = 7
              order by 1 desc;
            """;

        return await _connection.QueryAsync<EntregableRevisionResponse>(sql);
    }

    public async Task<EntregableResponseEstado?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
              SELECT 
                ent.idEntregable AS IdEntregable,
                ent.rutaEntregable AS RutaEntregable,
                ent.descripcionEntregable AS DescripcionEntregable,
                ent.idEjecucion AS IdEjecucion, 
            	ent.numeroEntrega as NumeroEntrega,
            	ent.rutaDACPAC as RutaDACPAC,
            	ent.temporalBD as TemporalBD,
                est.descripcionEstado as EstadoEntrega,
            	est.idEstado as EstadoEntregaId,
                ent.rutaResultado as RutaResultado,
                ent.rutaRollbackGenerado as RutaResultadoRollback
            FROM dbo.tbl_Entregables ent
            JOIN dbo.tbl_EstadoEntrega est
            on ent.idEstado = est.idEstado
            WHERE ent.idEntregable = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<EntregableResponseEstado>(sql, new { Id = id });
    }

    public async Task<int> GetEntregablesByEjecucion(decimal idEjecucion)
    {
        const string sql = """
            select count(1) from dbo.tbl_Entregables
            where idEjecucion = @Id
            """;

        return await _connection.ExecuteScalarAsync<int>(sql, new { Id = idEjecucion });
    }
}