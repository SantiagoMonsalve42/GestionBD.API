using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.CommandsHandlers;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Artefactos.CommandsHandlers;

public sealed class CreateArtefactoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<IArtefactoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        TblArtefacto? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblArtefacto>()))
            .Callback<TblArtefacto>(entity =>
            {
                entity.IdArtefacto = 10m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.Artefactos).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateArtefactoCommandHandler(unitOfWorkMock.Object);

        var request = new CreateArtefactoRequest(1m, 1, "UTF-8", "script.sql", "script.sql", false);
        var result = await handler.Handle(new CreateArtefactoCommand(request), CancellationToken.None);

        Assert.Equal(10m, result);
        Assert.NotNull(created);
        Assert.Equal("script.sql", created!.NombreArtefacto);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}