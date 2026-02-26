using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Ejecuciones;
using GestionBD.Application.Ejecuciones.Queries;
using GestionBD.Application.Ejecuciones.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Ejecuciones.QueriesHandlers;

public sealed class GetEjecucionByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new EjecucionResponse(1m, 1m, DateTime.UtcNow, DateTime.UtcNow, null, null);

        var repositoryMock = new Mock<IEjecucionReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetEjecucionByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetEjecucionByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}