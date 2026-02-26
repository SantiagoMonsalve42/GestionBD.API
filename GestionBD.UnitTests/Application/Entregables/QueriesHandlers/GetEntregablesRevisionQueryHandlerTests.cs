using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Queries;
using GestionBD.Application.Entregables.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.QueriesHandlers;

public sealed class GetEntregablesRevisionQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new EntregableRevisionResponse(1m, "p", "req", "desc", 1) };

        var repositoryMock = new Mock<IEntregableReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllRevisionesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetEntregablesRevisionQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetEntregablesRevisionQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}