using MediatR;
using GestionBD.Application.Contracts.Motores;
using System.Data;
using Dapper;

namespace GestionBD.Application.Motores.Queries;

public sealed class GetMotorByIdQueryHandler : IRequestHandler<GetMotorByIdQuery, MotorResponse?>
{
    private readonly IDbConnection _connection;

    public GetMotorByIdQueryHandler(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<MotorResponse?> Handle(GetMotorByIdQuery request, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT 
                idMotor AS IdMotor,
                nombreMotor AS NombreMotor,
                versionMotor AS VersionMotor,
                descripcionMotor AS DescripcionMotor
            FROM dbo.tbl_Motores
            WHERE idMotor = @IdMotor;
            """;

        var motor = await _connection.QuerySingleOrDefaultAsync<MotorResponse>(sql, new { request.IdMotor });
        return motor;
    }
}