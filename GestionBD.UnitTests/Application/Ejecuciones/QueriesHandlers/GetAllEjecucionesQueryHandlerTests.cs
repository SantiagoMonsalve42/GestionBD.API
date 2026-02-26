using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Ejecuciones;
using GestionBD.Application.Ejecuciones.Queries;
using GestionBD.Application.Ejecuciones.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Ejecuciones.QueriesHandlers;

public sealed class GetAllEjecucionesQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new EjecucionResponse(1m, 1m, DateTime.UtcNow, DateTime.UtcNow, null, null) };

        var repositoryMock = new Mock<IEjecucionReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllEjecucionesQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllEjecucionesQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}