using GestionBD.Application.DTO;

namespace GestionBD.Application.Abstractions.Repositories;
public interface IStatisticsRepository
{
    Task<DashboardStatisticsDto?> GetDashboardStatisticsAsync();
}
