using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Artefactos.Queries;
using GestionBD.Application.Artefactos.QueriesHandlers;
using GestionBD.Application.Contracts.Artefactos;
using Moq;

namespace GestionBD.UnitTests.Application.Artefactos.QueriesHandlers;

public sealed class GetArtefactoByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new ArtefactoResponse(1m, 1m, 1, "UTF-8", "a.sql", "a.sql", false, null);

        var repositoryMock = new Mock<IArtefactoReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetArtefactoByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetArtefactoByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}