using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Queries;
using GestionBD.Application.Motores.QueriesHandlers;
using Moq;

namespace GestionBD.UnitTests.Application.Motores.QueriesHandlers;

public sealed class GetMotorByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_RepositoryReturnsItem_ReturnsItem()
    {
        var expected = new MotorResponse(1m, "SQL", "16", "desc");

        var repositoryMock = new Mock<IMotorReadRepository>();
        repositoryMock
            .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetMotorByIdQueryHandler(repositoryMock.Object);

        var result = await handler.Handle(new GetMotorByIdQuery(1m), CancellationToken.None);

        Assert.Same(expected, result);
    }
}