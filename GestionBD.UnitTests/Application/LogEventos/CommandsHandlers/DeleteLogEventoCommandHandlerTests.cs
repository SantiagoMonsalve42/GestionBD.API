using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.LogEventos.Commands;
using GestionBD.Application.LogEventos.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.LogEventos.CommandsHandlers;

public sealed class DeleteLogEventoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingLogEvento_RemovesAndSaves()
    {
        var logEvento = new TblLogEvento { IdEvento = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblLogEvento>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logEvento);

        var repositoryMock = new Mock<ILogEventoRepository>();
        unitOfWorkMock.SetupGet(x => x.LogEventos).Returns(repositoryMock.Object);

        var handler = new DeleteLogEventoCommandHandler(unitOfWorkMock.Object);

        await handler.Handle(new DeleteLogEventoCommand(1m), CancellationToken.None);

        repositoryMock.Verify(x => x.Remove(logEvento), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LogEventoNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblLogEvento>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblLogEvento?)null);

        var handler = new DeleteLogEventoCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteLogEventoCommand(1m), CancellationToken.None));
    }
}