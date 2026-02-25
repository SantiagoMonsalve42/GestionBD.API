using Dapper;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Motores;
using System.Data;

namespace GestionBD.Infrastructure.Repositories.Query;

public sealed class MotorReadRepository : IMotorReadRepository
{
    private readonly IDbConnection _connection;

    public MotorReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<MotorResponse>> GetAllAsync(CancellationToken cancellationToken = default)
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

        return await _connection.QueryAsync<MotorResponse>(sql);
    }

    public async Task<MotorResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                idMotor AS IdMotor,
                nombreMotor AS NombreMotor,
                versionMotor AS VersionMotor,
                descripcionMotor AS DescripcionMotor
            FROM dbo.tbl_Motores
            WHERE idMotor = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<MotorResponse>(sql, new { Id = id });
    }
}