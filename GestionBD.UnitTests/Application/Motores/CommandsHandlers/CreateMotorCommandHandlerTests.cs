using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Commands;
using GestionBD.Application.Motores.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using Moq;

namespace GestionBD.UnitTests.Application.Motores.CommandsHandlers;

public sealed class CreateMotorCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsId()
    {
        var repositoryMock = new Mock<IMotorRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        TblMotore? created = null;
        repositoryMock
            .Setup(x => x.Add(It.IsAny<TblMotore>()))
            .Callback<TblMotore>(entity =>
            {
                entity.IdMotor = 2m;
                created = entity;
            });

        unitOfWorkMock.SetupGet(x => x.Motores).Returns(repositoryMock.Object);
        unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateMotorCommandHandler(unitOfWorkMock.Object);

        var request = new CreateMotorRequest("SQL", "16", "desc");
        var result = await handler.Handle(new CreateMotorCommand(request), CancellationToken.None);

        Assert.Equal(2m, result);
        Assert.Equal("SQL", created!.NombreMotor);
    }
}