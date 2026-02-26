using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;
using Moq;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class CreateEntregableCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<IEntregableRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        TblEntregable? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblEntregable>()))
            .Callback<TblEntregable>(entity =>
            {
                entity.IdEntregable = 11m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.Entregables).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateEntregableCommandHandler(unitOfWorkMock.Object);

        var request = new CreateEntregableRequest("path.zip", "desc", 1m, 1);
        var result = await handler.Handle(new CreateEntregableCommand(request), CancellationToken.None);

        Assert.Equal(11m, result);
        Assert.NotNull(created);
        Assert.Equal((int)EstadoEntregaEnum.Creado, created!.IdEstado);
    }
}