using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Parametros.Queries;
using GestionBD.Application.Parametros.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Parametros.QueriesHandlers;

public sealed class GetParametroByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new ParametroResponse(1m, "P", 1m, "v");

        var repositoryMock = new Mock<IParametroReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetParametroByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetParametroByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}