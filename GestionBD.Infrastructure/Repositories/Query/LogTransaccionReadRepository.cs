using Dapper;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogTransacciones;
using System.Data;

namespace GestionBD.Infrastructure.Repositories.Query;

public sealed class LogTransaccionReadRepository : ILogTransaccionReadRepository
{
    private readonly IDbConnection _connection;

    public LogTransaccionReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<LogTransaccionResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                idTransaccion AS IdTransaccion,
                nombreTransaccion AS NombreTransaccion,
                estadoTransaccion AS EstadoTransaccion,
                descripcionTransaccion AS DescripcionTransaccion,
                fechaInicio AS FechaInicio,
                respuestaTransaccion AS RespuestaTransaccion,
                fechaFin AS FechaFin,
                usuarioEjecucion AS UsuarioEjecucion
            FROM dbo.tbl_logTransacciones
            ORDER BY fechaInicio DESC;
            """;

        return await _connection.QueryAsync<LogTransaccionResponse>(sql);
    }

    public async Task<LogTransaccionResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT 
                idTransaccion AS IdTransaccion,
                nombreTransaccion AS NombreTransaccion,
                estadoTransaccion AS EstadoTransaccion,
                descripcionTransaccion AS DescripcionTransaccion,
                fechaInicio AS FechaInicio,
                respuestaTransaccion AS RespuestaTransaccion,
                fechaFin AS FechaFin,
                usuarioEjecucion AS UsuarioEjecucion
            FROM dbo.tbl_logTransacciones
            WHERE idTransaccion = @Id;
            """;

        return await _connection.QueryFirstOrDefaultAsync<LogTransaccionResponse>(sql, new { Id = id });
    }
}