using MediatR;
using GestionBD.Application.Contracts.Parametros;
using System.Data;
using Dapper;

namespace GestionBD.Application.Parametros.Queries;

public sealed class GetParametroByIdQueryHandler : IRequestHandler<GetParametroByIdQuery, ParametroResponse?>
{
    private readonly IDbConnection _connection;

    public GetParametroByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ParametroResponse?> Handle(GetParametroByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                idParametro AS IdParametro,
                nombreParametro AS NombreParametro,
                valorNumerico AS ValorNumerico,
                valorString AS ValorString
            FROM dbo.tbl_Parametros
            WHERE idParametro = @IdParametro;
            """;

        var parametro = await _connection.QuerySingleOrDefaultAsync<ParametroResponse>(sql, new { request.IdParametro });
        return parametro;
    }
}