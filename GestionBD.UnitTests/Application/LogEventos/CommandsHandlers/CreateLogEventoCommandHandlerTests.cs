using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Commands;
using GestionBD.Application.LogEventos.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.LogEventos.CommandsHandlers;

public sealed class CreateLogEventoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<ILogEventoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        TblLogEvento? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblLogEvento>()))
            .Callback<TblLogEvento>(entity =>
            {
                entity.IdEvento = 9m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.LogEventos).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateLogEventoCommandHandler(unitOfWorkMock.Object);

        var request = new CreateLogEventoRequest(1m, DateTime.UtcNow, "desc", "ok");
        var result = await handler.Handle(new CreateLogEventoCommand(request), CancellationToken.None);

        Assert.Equal(9m, result);
        Assert.Equal("desc", created!.Descripcion);
    }
}