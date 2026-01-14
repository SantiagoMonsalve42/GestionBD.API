using System.Data;
using Dapper;
using GestionBD.Application.Abstractions.Readers;
using GestionBD.Application.Contracts.Instancias;

namespace GestionBD.Infraestructure.Repositories.Query;

public sealed class InstanciaReadRepository : IInstanciaReadRepository
{
    private readonly IDbConnection _connection;

    public InstanciaReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<InstanciaResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                i.idInstancia AS IdInstancia,
                i.idMotor AS IdMotor,
                i.instancia AS Instancia,
                i.puerto AS Puerto,
                i.usuario AS Usuario,
                i.nombreBD AS NombreBD
            FROM dbo.tbl_Instancias i
            ORDER BY i.instancia;
            """;

        return await _connection.QueryAsync<InstanciaResponse>(sql);
    }

    public async Task<InstanciaResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                i.idInstancia AS IdInstancia,
                i.idMotor AS IdMotor,
                i.instancia AS Instancia,
                i.puerto AS Puerto,
                i.usuario AS Usuario,
                i.nombreBD AS NombreBD
            FROM dbo.tbl_Instancias i
            WHERE i.idInstancia = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<InstanciaResponse>(sql, new { Id = id });
    }

    public async Task<InstanciaConnectResponse?> GetConnectionDetailsByEntregableIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            select 
            	i.instancia AS Instancia,
            	i.usuario AS Usuario,
            	i.password AS Password,
            	i.puerto AS Puerto,
            	i.nombreBD AS NombreBD,
                en.temporalBD AS TemporalBD
              from [dbo].[tbl_Instancias] i
              join dbo.tbl_Ejecuciones e
              on i.idInstancia = e.idEjecucion
              join dbo.tbl_Entregables en
              on en.idEjecucion = e.idEjecucion
              where en.idEntregable = @Id
            """;

        return await _connection.QueryFirstOrDefaultAsync<InstanciaConnectResponse>(sql, new { Id = id });
    }
}