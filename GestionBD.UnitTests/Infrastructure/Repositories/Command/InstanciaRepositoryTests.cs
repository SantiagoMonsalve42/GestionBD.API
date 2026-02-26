using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Infrastructure.Data;
using GestionBD.Infrastructure.Repositories.Command;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Command;

public sealed class InstanciaRepositoryTests
{
    [Fact]
    public void Create_ValidContext_ReturnsRepository()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var contextMock = new Mock<ApplicationDbContext>(options);

        var repository = new InstanciaRepository(contextMock.Object);

        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IInstanciaRepository>(repository);
    }
}