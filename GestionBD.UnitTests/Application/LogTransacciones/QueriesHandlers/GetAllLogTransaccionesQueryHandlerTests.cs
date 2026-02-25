using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogTransacciones;
using GestionBD.Application.LogTransacciones.Queries;
using GestionBD.Application.LogTransacciones.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.LogTransacciones.QueriesHandlers;

public sealed class GetAllLogTransaccionesQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new LogTransaccionResponse(1m, "n", "A", "d", DateTime.UtcNow, null, null, "u") };

        var repositoryMock = new Mock<ILogTransaccionReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllLogTransaccionesQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllLogTransaccionesQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}