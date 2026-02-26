using GestionBD.Infrastructure.Repositories.Query;
using GestionBD.UnitTests.Infrastructure.Repositories.Query.Fakes;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Query;

public sealed class InstanciaReadRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_NoRows_ReturnsEmpty()
    {
        var connection = new FakeDbConnection();
        var repository = new InstanciaReadRepository(connection);

        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetConnectionDetailsByEntregableIdAsync_NoRows_ReturnsNull()
    {
        var connection = new FakeDbConnection();
        var repository = new InstanciaReadRepository(connection);

        var result = await repository.GetConnectionDetailsByEntregableIdAsync(1m);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ConnectionFails_ThrowsInvalidOperationException()
    {
        var connection = new FakeDbConnection { ThrowOnExecute = true };
        var repository = new InstanciaReadRepository(connection);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetAllAsync());
    }
}