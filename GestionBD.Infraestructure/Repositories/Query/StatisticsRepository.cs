using Dapper;
using GestionBD.Application.Abstractions.Repositories;
using GestionBD.Application.DTO;
using System.Data;

namespace GestionBD.Infraestructure.Repositories.Query;

internal class StatisticsRepository : IStatisticsRepository
{
    private readonly IDbConnection _connection;

    public StatisticsRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<DashboardStatisticsDto?> GetDashboardStatisticsAsync()
    {
        const string query = @"
                SELECT 
                    (SELECT COUNT(1) FROM [dbo].[tbl_Artefactos]) AS CantidadArtefactos,
                    (SELECT COUNT(1) FROM [dbo].[tbl_Entregables]) AS CantidadEntregables,
                    (SELECT COUNT(1) FROM [dbo].[tbl_Instancias]) AS CantidadInstancias";

        return await _connection.QueryFirstOrDefaultAsync<DashboardStatisticsDto>(query);
    }
}
