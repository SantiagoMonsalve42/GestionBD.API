using MediatR;
using GestionBD.Application.Contracts.Instancias;
using System.Data;
using Dapper;

namespace GestionBD.Application.Instancias.Queries;

public sealed class GetInstanciaByIdQueryHandler : IRequestHandler<GetInstanciaByIdQuery, InstanciaResponse?>
{
    private readonly IDbConnection _connection;

    public GetInstanciaByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<InstanciaResponse?> Handle(GetInstanciaByIdQuery request, CancellationToken cancellationToken)
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
            WHERE i.idInstancia = @IdInstancia;
            """;

        var instancia = await _connection.QuerySingleOrDefaultAsync<InstanciaResponse>(sql, new { request.IdInstancia });
        return instancia;
    }
}