using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class EntregableToRevisionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_UpdatesEstado()
    {
        var repositoryMock = new Mock<IEntregableRepository>();
        repositoryMock
            .Setup(x => x.UpdateEstado(1m, EstadoEntregaEnum.Revision, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.SetupGet(x => x.Entregables).Returns(repositoryMock.Object);

        var handler = new EntregableToRevisionCommandHandler(unitOfWorkMock.Object);

        await handler.Handle(new EntregableToRevisionCommand(1m), CancellationToken.None);

        repositoryMock.Verify(x => x.UpdateEstado(1m, EstadoEntregaEnum.Revision, It.IsAny<CancellationToken>()), Times.Once);
    }
}