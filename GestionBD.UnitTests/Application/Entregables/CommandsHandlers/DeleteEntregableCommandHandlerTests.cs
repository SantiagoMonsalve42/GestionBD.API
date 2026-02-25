using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class DeleteEntregableCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingEntregable_RemovesAndDeletesFile()
    {
        var entregable = new TblEntregable { IdEntregable = 1m, RutaEntregable = "file.zip" };

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblEntregable>(1m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entregable);

        var entregableRepositoryMock = new Mock<IEntregableRepository>();
        unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

        var fileStorageMock = new Mock<IFileStorageService>();

        var handler = new DeleteEntregableCommandHandler(unitOfWorkMock.Object, fileStorageMock.Object);

        await handler.Handle(new DeleteEntregableCommand(1m), CancellationToken.None);

        fileStorageMock.Verify(x => x.DeleteFileAsync("file.zip", It.IsAny<CancellationToken>()), Times.Once);
        entregableRepositoryMock.Verify(x => x.Remove(entregable), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EntregableNotFound_ThrowsKeyNotFoundException()
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(x => x.FindEntityAsync<TblEntregable>(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TblEntregable?)null);

        var handler = new DeleteEntregableCommandHandler(unitOfWorkMock.Object, Mock.Of<IFileStorageService>());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteEntregableCommand(1m), CancellationToken.None));
    }
}