using MediatR;
using GestionBD.Application.Contracts.Motores;
using System.Data;
using Dapper;

namespace GestionBD.Application.Motores.Queries;

public sealed class GetAllMotoresQueryHandler : IRequestHandler<GetAllMotoresQuery, IEnumerable<MotorResponse>>
{
    private readonly IDbConnection _connection;

    public GetAllMotoresQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<MotorResponse>> Handle(GetAllMotoresQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                idMotor AS IdMotor,
                nombreMotor AS NombreMotor,
                versionMotor AS VersionMotor,
                descripcionMotor AS DescripcionMotor
            FROM dbo.tbl_Motores
            ORDER BY nombreMotor;
            """;

        var motores = await _connection.QueryAsync<MotorResponse>(sql);
        return motores;
    }
}