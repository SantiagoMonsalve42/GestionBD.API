using MediatR;
using GestionBD.Application.Contracts.Instancias;
using System.Data;
using Dapper;

namespace GestionBD.Application.Instancias.Queries;

public sealed class GetAllInstanciasQueryHandler : IRequestHandler<GetAllInstanciasQuery, IEnumerable<InstanciaResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllInstanciasQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<InstanciaResponse>> Handle(GetAllInstanciasQuery request, CancellationToken cancellationToken)
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

        var instancias = await _connection.QueryAsync<InstanciaResponse>(sql);
        return instancias;
    }
}