using System.Data;
using Dapper;
using GestionBD.Application.Abstractions;
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
                m.nombreMotor AS NombreMotor
            FROM dbo.tbl_Instancias i
            INNER JOIN dbo.tbl_Motores m ON i.idMotor = m.idMotor
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
                m.nombreMotor AS NombreMotor
            FROM dbo.tbl_Instancias i
            INNER JOIN dbo.tbl_Motores m ON i.idMotor = m.idMotor
            WHERE i.idInstancia = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<InstanciaResponse>(sql, new { Id = id });
    }
}