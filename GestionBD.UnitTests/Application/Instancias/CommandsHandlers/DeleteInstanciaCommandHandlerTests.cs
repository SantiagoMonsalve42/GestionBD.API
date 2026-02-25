using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Instancias.CommandsHandlers;

public sealed class DeleteInstanciaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingInstancia_RemovesAndSaves()
    {
        var instancia = new TblInstancia { IdInstancia = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblInstancia>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instancia);

        var repositoryMock = new Mock<IInstanciaRepository>();
        unitOfWorkMock.SetupGet(x => x.Instancias).Returns(repositoryMock.Object);

        var handler = new DeleteInstanciaCommandHandler(unitOfWorkMock.Object);

        await handler.Handle(new DeleteInstanciaCommand(1m), CancellationToken.None);

        repositoryMock.Verify(x => x.Remove(instancia), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InstanciaNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblInstancia>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblInstancia?)null);

        var handler = new DeleteInstanciaCommandHandler(unitOfWorkMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteInstanciaCommand(1m), CancellationToken.None));
    }
}