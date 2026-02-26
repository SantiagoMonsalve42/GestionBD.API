using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.DTO;
using GestionBD.Application.Statistics.Queries;
using GestionBD.Application.Statistics.QueriesHandler;
using Moq;

namespace GestionBD.UnitTests.Application.Statistics.QueriesHandler;

public sealed class GetDashboardStatisticsQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new DashboardStatisticsDto(1, 2, 3);

        var repositoryMock = new Mock<IStatisticsRepository>();
        repositoryMock
            .Setup(x => x.GetDashboardStatisticsAsync())
            .ReturnsAsync(expected);

        var handler = new GetDashboardStatisticsQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetDashboardStatisticsQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}