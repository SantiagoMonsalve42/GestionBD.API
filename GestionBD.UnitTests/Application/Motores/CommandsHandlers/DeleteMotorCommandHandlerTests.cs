using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Motores.Commands;
using GestionBD.Application.Motores.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Motores.CommandsHandlers;

public sealed class DeleteMotorCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingMotor_RemovesAndSaves()
    {
        var motor = new TblMotore { IdMotor = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblMotore>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motor);

        var repositoryMock = new Mock<IMotorRepository>();
        unitOfWorkMock.SetupGet(x => x.Motores).Returns(repositoryMock.Object);

        var handler = new DeleteMotorCommandHandler(unitOfWorkMock.Object);

        await handler.Handle(new DeleteMotorCommand(1m), CancellationToken.None);

        repositoryMock.Verify(x => x.Remove(motor), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MotorNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblMotore>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblMotore?)null);

        var handler = new DeleteMotorCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteMotorCommand(1m), CancellationToken.None));
    }
}