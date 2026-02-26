using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Queries;
using GestionBD.Application.LogEventos.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.LogEventos.QueriesHandlers;

public sealed class GetLogEventoByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new LogEventoResponse(1m, 1m, DateTime.UtcNow, "d", "ok", null);

        var repositoryMock = new Mock<ILogEventoReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetLogEventoByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetLogEventoByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}