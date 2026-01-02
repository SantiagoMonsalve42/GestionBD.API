using MediatR;
using GestionBD.Application.Contracts.Parametros;
using System.Data;
using Dapper;

namespace GestionBD.Application.Parametros.Queries;

public sealed class GetAllParametrosQueryHandler : IRequestHandler<GetAllParametrosQuery, IEnumerable<ParametroResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllParametrosQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<ParametroResponse>> Handle(GetAllParametrosQuery request, CancellationToken cancellationToken)
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

        var parametros = await _connection.QueryAsync<ParametroResponse>(sql);
        return parametros;
    }
}