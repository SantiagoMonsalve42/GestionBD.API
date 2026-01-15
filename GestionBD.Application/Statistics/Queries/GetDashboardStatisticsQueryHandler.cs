using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.DTO;
using MediatR;

namespace GestionBD.Application.Statistics.Queries;

public sealed class GetDashboardStatisticsQueryHandler
        : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsDto?>
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetDashboardStatisticsQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository
            ?? throw new ArgumentNullException(nameof(statisticsRepository));
    }

    public async Task<DashboardStatisticsDto?> Handle(
        GetDashboardStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        return await _statisticsRepository.GetDashboardStatisticsAsync();
    }
}
