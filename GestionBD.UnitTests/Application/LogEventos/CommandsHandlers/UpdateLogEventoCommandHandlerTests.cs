using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Commands;
using GestionBD.Application.LogEventos.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.LogEventos.CommandsHandlers;

public sealed class UpdateLogEventoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingLogEvento_UpdatesAndSaves()
    {
        var logEvento = new TblLogEvento { IdEvento = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblLogEvento>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logEvento);

        var logEventoRepositoryMock = new Mock<ILogEventoRepository>();
        unitOfWorkMock.SetupGet(x => x.LogEventos).Returns(logEventoRepositoryMock.Object);

        var handler = new UpdateLogEventoCommandHandler(unitOfWorkMock.Object);

        var request = new UpdateLogEventoRequest(1m, 2m, DateTime.UtcNow, "Desc", "OK");
        await handler.Handle(new UpdateLogEventoCommand(request), CancellationToken.None);

        Assert.Equal("Desc", logEvento.Descripcion);
        logEventoRepositoryMock.Verify(x => x.Update(logEvento), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LogEventoNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblLogEvento>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblLogEvento?)null);

        var handler = new UpdateLogEventoCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateLogEventoCommand(new UpdateLogEventoRequest(1m, 2m, DateTime.UtcNow, "Desc", "OK")), CancellationToken.None));
    }
}