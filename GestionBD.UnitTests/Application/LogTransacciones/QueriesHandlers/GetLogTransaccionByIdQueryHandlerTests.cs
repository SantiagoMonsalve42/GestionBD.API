using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogTransacciones;
using GestionBD.Application.LogTransacciones.Queries;
using GestionBD.Application.LogTransacciones.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.LogTransacciones.QueriesHandlers;

public sealed class GetLogTransaccionByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new LogTransaccionResponse(1m, "n", "A", "d", DateTime.UtcNow, null, null, "u");

        var repositoryMock = new Mock<ILogTransaccionReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetLogTransaccionByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetLogTransaccionByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}