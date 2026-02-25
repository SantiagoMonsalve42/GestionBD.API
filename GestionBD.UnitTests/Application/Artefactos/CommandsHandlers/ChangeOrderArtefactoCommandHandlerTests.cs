using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.CommandsHandlers;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Domain;
using Moq;

namespace GestionBD.UnitTests.Application.Artefactos.CommandsHandlers;

public sealed class ChangeOrderArtefactoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsTrue()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var artefactoRepositoryMock = new Mock<IArtefactoRepository>();

        unitOfWorkMock.SetupGet(x => x.Artefactos).Returns(artefactoRepositoryMock.Object);
        artefactoRepositoryMock
            .Setup(x => x.UpdateOrder(It.IsAny<Dictionary<decimal, int>>()))
            .ReturnsAsync(true);

        var handler = new ChangeOrderArtefactoCommandHandler(unitOfWorkMock.Object);

        var result = await handler.Handle(
            new ChangeOrderArtefactoCommand([new ArtefactoChangeOrder(1m, 1)]),
            CancellationToken.None);

        Assert.True(result);
        unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateOrderThrows_ReturnsFalse()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var artefactoRepositoryMock = new Mock<IArtefactoRepository>();

        unitOfWorkMock.SetupGet(x => x.Artefactos).Returns(artefactoRepositoryMock.Object);
        artefactoRepositoryMock
            .Setup(x => x.UpdateOrder(It.IsAny<Dictionary<decimal, int>>()))
            .ThrowsAsync(new InvalidOperationException());

        var handler = new ChangeOrderArtefactoCommandHandler(unitOfWorkMock.Object);

        var result = await handler.Handle(
            new ChangeOrderArtefactoCommand([new ArtefactoChangeOrder(1m, 1)]),
            CancellationToken.None);

        Assert.False(result);
        unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}