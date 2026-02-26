using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Queries;
using GestionBD.Application.LogEventos.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.LogEventos.QueriesHandlers;

public sealed class GetAllLogEventosQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new LogEventoResponse(1m, 1m, DateTime.UtcNow, "d", "ok", null) };

        var repositoryMock = new Mock<ILogEventoReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllLogEventosQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllLogEventosQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}