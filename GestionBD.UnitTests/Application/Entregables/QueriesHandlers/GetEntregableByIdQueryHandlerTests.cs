using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Queries;
using GestionBD.Application.Entregables.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.QueriesHandlers;

public sealed class GetEntregableByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new EntregableResponseEstado(1m, "p", "d", 1m, 1, null, null, "e", 1, null, null);

        var repositoryMock = new Mock<IEntregableReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetEntregableByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetEntregableByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}