using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Parametros.Commands;
using GestionBD.Application.Parametros.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Parametros.CommandsHandlers;

public sealed class CreateParametroCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<IParametroRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        TblParametro? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblParametro>()))
            .Callback<TblParametro>(entity =>
            {
                entity.IdParametro = 7m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.Parametros).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateParametroCommandHandler(unitOfWorkMock.Object);

        var request = new CreateParametroRequest("Param", 1m, "v");
        var result = await handler.Handle(new CreateParametroCommand(request), CancellationToken.None);

        Assert.Equal(7m, result);
        Assert.Equal("Param", created!.NombreParametro);
    }
}