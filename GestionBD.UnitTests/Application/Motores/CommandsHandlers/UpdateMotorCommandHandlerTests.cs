using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Commands;
using GestionBD.Application.Motores.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Motores.CommandsHandlers;

public sealed class UpdateMotorCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingMotor_UpdatesAndSaves()
    {
        var motor = new TblMotore { IdMotor = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblMotore>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motor);

        var motorRepositoryMock = new Mock<IMotorRepository>();
        unitOfWorkMock.SetupGet(x => x.Motores).Returns(motorRepositoryMock.Object);

        var handler = new UpdateMotorCommandHandler(unitOfWorkMock.Object);

        var request = new UpdateMotorRequest(1m, "SQL", "16", "Desc");
        await handler.Handle(new UpdateMotorCommand(request), CancellationToken.None);

        Assert.Equal("SQL", motor.NombreMotor);
        motorRepositoryMock.Verify(x => x.Update(motor), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MotorNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblMotore>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblMotore?)null);

        var handler = new UpdateMotorCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateMotorCommand(new UpdateMotorRequest(1m, "SQL", "16", "Desc")), CancellationToken.None));
    }
}