using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Artefactos.CommandsHandlers;

public sealed class DeleteArtefactoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingArtefacto_RemovesAndSaves()
    {
        var artefacto = new TblArtefacto { IdArtefacto = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblArtefacto>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artefacto);

        var repositoryMock = new Mock<IArtefactoRepository>();
        unitOfWorkMock.SetupGet(x => x.Artefactos).Returns(repositoryMock.Object);

        var handler = new DeleteArtefactoCommandHandler(unitOfWorkMock.Object);

        await handler.Handle(new DeleteArtefactoCommand(1m), CancellationToken.None);

        repositoryMock.Verify(x => x.Remove(artefacto), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ArtefactoNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblArtefacto>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblArtefacto?)null);

        var handler = new DeleteArtefactoCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteArtefactoCommand(1m), CancellationToken.None));
    }
}