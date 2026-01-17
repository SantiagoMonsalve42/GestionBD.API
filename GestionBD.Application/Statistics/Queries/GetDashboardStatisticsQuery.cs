using GestionBD.Application.DTO;
using MediatR;

namespace GestionBD.Application.Statistics.Queries;

public record GetDashboardStatisticsQuery : IRequest<DashboardStatisticsDto?>;

