using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Parametros.Queries;
using GestionBD.Application.Parametros.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Parametros.QueriesHandlers;

public sealed class GetAllParametrosQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new ParametroResponse(1m, "P", 1m, "v") };

        var repositoryMock = new Mock<IParametroReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllParametrosQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllParametrosQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}