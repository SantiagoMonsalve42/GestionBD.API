using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Queries;
using GestionBD.Application.Instancias.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Instancias.QueriesHandlers;

public sealed class GetAllInstanciasQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new InstanciaResponse(1m, 1m, "srv", 1433, "db") };

        var repositoryMock = new Mock<IInstanciaReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllInstanciasQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllInstanciasQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}