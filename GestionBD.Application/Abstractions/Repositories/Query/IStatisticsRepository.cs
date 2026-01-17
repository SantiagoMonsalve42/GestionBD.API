using GestionBD.Application.DTO;

namespace GestionBD.Application.Abstractions.Repositories.Query;
public interface IStatisticsRepository
{
    Task<DashboardStatisticsDto?> GetDashboardStatisticsAsync();
}
