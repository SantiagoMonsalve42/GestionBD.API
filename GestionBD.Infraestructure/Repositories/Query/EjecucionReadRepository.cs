using System.Data;
using Dapper;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Infraestructure.Repositories.Query;

public sealed class EjecucionReadRepository : IEjecucionReadRepository
{
    private readonly IDbConnection _connection;

    public EjecucionReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<EjecucionResponse>> GetAllAsync(CancellationToken cancellationToken = default)
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

        return await _connection.QueryAsync<EjecucionResponse>(sql);
    }

    public async Task<EjecucionResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
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
            WHERE e.idEjecucion = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<EjecucionResponse>(sql, new { Id = id });
    }
}