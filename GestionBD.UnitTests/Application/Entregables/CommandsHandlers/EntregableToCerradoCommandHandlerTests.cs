using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class EntregableToCerradoCommandHandlerTests
{
    [Fact]
    public async Task Handle_CloseRequest_UpdatesEstadoAndDropsTempDb()
    {
        var entregableId = 4m;
        var entregable = new EntregableResponseEstado(
            entregableId,
            "path.zip",
            "Desc",
            1m,
            1,
            null,
            "tempdb",
            "Estado",
            1,
            null,
            null);

        var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
        entregableReadRepositoryMock
            .Setup(x => x.GetByIdAsync(entregableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregable);

        var entregableRepositoryMock = new Mock<IEntregableRepository>();
        entregableRepositoryMock
            .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.Cerrado, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        entregableRepositoryMock
            .Setup(x => x.removeDatabase(entregableId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

        var dacpacServiceMock = new Mock<IDacpacService>();

        var handler = new EntregableToCerradoCommandHandler(
            unitOfWorkMock.Object,
            entregableReadRepositoryMock.Object,
            dacpacServiceMock.Object);

        await handler.Handle(new EntregableToCerradoCommand(entregableId, 1), CancellationToken.None);

        dacpacServiceMock.Verify(x => x.DropTemporaryDatabaseAsync("tempdb", It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EntregableNotFound_ThrowsException()
    {
        var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
        entregableReadRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntregableResponseEstado?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new EntregableToCerradoCommandHandler(
            unitOfWorkMock.Object,
            entregableReadRepositoryMock.Object,
            Mock.Of<IDacpacService>());

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            handler.Handle(new EntregableToCerradoCommand(1m, 1), CancellationToken.None));

        Assert.Contains("Error al actualizar el estado del entregable", exception.Message);
        unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}