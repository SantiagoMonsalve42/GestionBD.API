using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Instancias.CommandsHandlers;

public sealed class CreateInstanciaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<IInstanciaRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var vaultConfigurationProviderMock = new Mock<IVaultConfigurationProvider>();

        TblInstancia? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblInstancia>()))
            .Callback<TblInstancia>(entity =>
            {
                entity.IdInstancia = 5m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.Instancias).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateInstanciaCommandHandler(unitOfWorkMock.Object, vaultConfigurationProviderMock.Object);

        var request = new CreateInstanciaRequest(1m, "srv", 1433, "usr", "pwd", "db");
        var result = await handler.Handle(new CreateInstanciaCommand(request), CancellationToken.None);

        Assert.Equal(5m, result);
        Assert.Equal("srv", created!.Instancia);
    }
}