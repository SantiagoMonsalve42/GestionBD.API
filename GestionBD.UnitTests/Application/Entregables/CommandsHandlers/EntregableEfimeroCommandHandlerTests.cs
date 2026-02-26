using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class EntregableEfimeroCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_UpdatesDacpacAndReturnsMessage()
    {
        var entregableId = 2m;

        var instanciaReadRepositoryMock = new Mock<IInstanciaReadRepository>();
        instanciaReadRepositoryMock
            .Setup(x => x.GetConnectionDetailsByEntregableIdAsync(entregableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InstanciaConnectResponse("srv", "usr", 1433, "db", "tempdb"));

        var dacpacServiceMock = new Mock<IDacpacService>();
        dacpacServiceMock
            .Setup(x => x.ExtractDacpacAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("dacpacPath");
        dacpacServiceMock
            .Setup(x => x.DeployDacpacToTemporaryDatabaseAsync("dacpacPath", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync("tempdb2");

        var entregableRepositoryMock = new Mock<IEntregableRepository>();
        entregableRepositoryMock
            .Setup(x => x.UpdateDACPAC(entregableId, "dacpacPath", "tempdb2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        entregableRepositoryMock
            .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.Preparacion, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
        keyVaultProvider
                .Setup(x => x.GetSecretsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, object>
                {
                    ["user"] = "usr",
                    ["pass"] = "pwd"
                });
        unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

        var handler = new EntregableEfimeroCommandHandler(
            unitOfWorkMock.Object,
            instanciaReadRepositoryMock.Object,
            dacpacServiceMock.Object,
            keyVaultProvider.Object);

        var result = await handler.Handle(new EntregableEfimeroCommand(entregableId), CancellationToken.None);

        Assert.Contains("dacpacPath", result);
        Assert.Contains("tempdb2", result);
        unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MissingConnectionDetails_ThrowsInvalidOperationException()
    {
        var instanciaReadRepositoryMock = new Mock<IInstanciaReadRepository>();
        instanciaReadRepositoryMock
            .Setup(x => x.GetConnectionDetailsByEntregableIdAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InstanciaConnectResponse?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
        var handler = new EntregableEfimeroCommandHandler(
            unitOfWorkMock.Object,
            instanciaReadRepositoryMock.Object,
            Mock.Of<IDacpacService>(),
            keyVaultProvider.Object);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new EntregableEfimeroCommand(1m), CancellationToken.None));

        Assert.Equal("No se pudo iniciar la transaccion.", exception.Message);
        unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}