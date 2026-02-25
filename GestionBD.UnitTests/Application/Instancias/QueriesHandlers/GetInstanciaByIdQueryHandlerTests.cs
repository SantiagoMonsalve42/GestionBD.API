using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Queries;
using GestionBD.Application.Instancias.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Instancias.QueriesHandlers;

public sealed class GetInstanciaByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new InstanciaResponse(1m, 1m, "srv", 1433, "usr", "db");

        var repositoryMock = new Mock<IInstanciaReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetInstanciaByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetInstanciaByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}