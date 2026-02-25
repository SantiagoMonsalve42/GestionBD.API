using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.CommandsHandlers;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Artefactos.CommandsHandlers;

public sealed class UpdateArtefactoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingArtefacto_UpdatesAndSaves()
    {
        var artefacto = new TblArtefacto { IdArtefacto = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblArtefacto>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(artefacto);

        var artefactoRepositoryMock = new Mock<IArtefactoRepository>();
        unitOfWorkMock.SetupGet(x => x.Artefactos).Returns(artefactoRepositoryMock.Object);

        var handler = new UpdateArtefactoCommandHandler(unitOfWorkMock.Object);

        var request = new UpdateArtefactoRequest(1m, 2m, 3, "UTF-8", "name.sql", "path.sql", false);
        await handler.Handle(new UpdateArtefactoCommand(request), CancellationToken.None);

        Assert.Equal("name.sql", artefacto.NombreArtefacto);
        artefactoRepositoryMock.Verify(x => x.Update(artefacto), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ArtefactoNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblArtefacto>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblArtefacto?)null);

        var handler = new UpdateArtefactoCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateArtefactoCommand(new UpdateArtefactoRequest(1m, 2m, 3, "UTF-8", "name", "path", false)), CancellationToken.None));
    }
}