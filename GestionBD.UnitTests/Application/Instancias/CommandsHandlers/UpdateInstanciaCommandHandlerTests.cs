using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Instancias.CommandsHandlers;

public sealed class UpdateInstanciaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingInstancia_UpdatesAndSaves()
    {
        var instancia = new TblInstancia { IdInstancia = 1m };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var vaultConfigurationProviderMock = new Mock<IVaultConfigurationProvider>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblInstancia>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instancia);

        var instanciaRepositoryMock = new Mock<IInstanciaRepository>();
        unitOfWorkMock.SetupGet(x => x.Instancias).Returns(instanciaRepositoryMock.Object);

        var handler = new UpdateInstanciaCommandHandler(unitOfWorkMock.Object, vaultConfigurationProviderMock.Object);

        var request = new UpdateInstanciaRequest(1m, 2m, "srv", 1433, "usr", "pwd", "db");
        await handler.Handle(new UpdateInstanciaCommand(request), CancellationToken.None);

        Assert.Equal("srv", instancia.Instancia);
        instanciaRepositoryMock.Verify(x => x.Update(instancia), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InstanciaNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblInstancia>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblInstancia?)null);

        var vaultConfigurationProviderMock = new Mock<IVaultConfigurationProvider>();
        var handler = new UpdateInstanciaCommandHandler(unitOfWorkMock.Object, vaultConfigurationProviderMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateInstanciaCommand(new UpdateInstanciaRequest(1m, 2m, "srv", 1433, "usr", "pwd", "db")), CancellationToken.None));
    }
}