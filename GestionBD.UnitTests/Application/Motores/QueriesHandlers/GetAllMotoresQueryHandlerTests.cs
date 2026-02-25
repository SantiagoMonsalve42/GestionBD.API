using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Queries;
using GestionBD.Application.Motores.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Motores.QueriesHandlers;

public sealed class GetAllMotoresQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItems_ReturnsItems()
    {
        var expected = new[] { new MotorResponse(1m, "SQL", "16", "desc") };

        var repositoryMock = new Mock<IMotorReadRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetAllMotoresQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetAllMotoresQuery(), CancellationToken.None);

        Assert.Same(expected, result);
    }
}