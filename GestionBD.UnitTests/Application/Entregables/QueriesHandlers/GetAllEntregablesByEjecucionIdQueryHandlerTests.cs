using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Queries;
using GestionBD.Application.Entregables.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.QueriesHandlers;

public sealed class GetAllEntregablesByEjecucionIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new EntregableResponseEstado(1m, "p", "d", 1m, 1, null, null, "e", 1, null, null) };

        var repositoryMock = new Mock<IEntregableReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllByIdEjecucionAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllEntregablesByEjecucionIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllEntregablesByEjecucionIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}