using System.Data;
using Dapper;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Parametros;

namespace GestionBD.Infrastructure.Repositories.Query;

public sealed class ParametroReadRepository : IParametroReadRepository
{
    private readonly IDbConnection _connection;

    public ParametroReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<ParametroResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                idParametro AS IdParametro,
                nombreParametro AS NombreParametro,
                valorNumerico AS ValorNumerico,
                valorString AS ValorString
            FROM dbo.tbl_Parametros
            ORDER BY nombreParametro;
            """;

        return await _connection.QueryAsync<ParametroResponse>(sql);
    }

    public async Task<ParametroResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                idParametro AS IdParametro,
                nombreParametro AS NombreParametro,
                valorNumerico AS ValorNumerico,
                valorString AS ValorString
            FROM dbo.tbl_Parametros
            WHERE idParametro = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<ParametroResponse>(sql, new { Id = id });
    }
}
