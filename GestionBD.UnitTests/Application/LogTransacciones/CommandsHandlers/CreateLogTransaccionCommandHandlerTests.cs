using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.LogTransacciones;
using GestionBD.Application.LogTransacciones.Commands;
using GestionBD.Application.LogTransacciones.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.LogTransacciones.CommandsHandlers;

public sealed class CreateLogTransaccionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<ILogTransaccionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        TblLogTransaccione? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblLogTransaccione>()))
            .Callback<TblLogTransaccione>(entity =>
            {
                entity.IdTransaccion = 4m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.LogTransacciones).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateLogTransaccionCommandHandler(unitOfWorkMock.Object);

        var request = new CreateLogTransaccionRequest("name", "A", "desc", DateTime.UtcNow, null, null, "user");
        var result = await handler.Handle(new CreateLogTransaccionCommand(request), CancellationToken.None);

        Assert.Equal(4m, result);
        Assert.Equal("name", created!.NombreTransaccion);
    }
}