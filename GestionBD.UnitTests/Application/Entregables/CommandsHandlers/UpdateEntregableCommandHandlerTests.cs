using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class UpdateEntregableCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingEntregable_UpdatesAndSaves()
    {
        var entregable = new TblEntregable { IdEntregable = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblEntregable>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregable);

        var repositoryMock = new Mock<IEntregableRepository>();
        unitOfWorkMock.SetupGet(x => x.Entregables).Returns(repositoryMock.Object);

        var handler = new UpdateEntregableCommandHandler(unitOfWorkMock.Object);

        var request = new UpdateEntregableRequest(1m, "path.zip", "desc", 1m);
        await handler.Handle(new UpdateEntregableCommand(request), CancellationToken.None);

        Assert.Equal("path.zip", entregable.RutaEntregable);
        repositoryMock.Verify(x => x.Update(entregable), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EntregableNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblEntregable>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblEntregable?)null);

        var handler = new UpdateEntregableCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateEntregableCommand(new UpdateEntregableRequest(1m, "p", "d", 1m)), CancellationToken.None));
    }
}