using GestionBD.Infrastructure.Repositories.Query;
using GestionBD.UnitTests.Infrastructure.Repositories.Query.Fakes;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Query;

public sealed class EntregableReadRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_NoRows_ReturnsEmpty()
    {
        var connection = new FakeDbConnection();
        var repository = new EntregableReadRepository(connection);

        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEntregablesByEjecucion_ScalarResult_ReturnsValue()
    {
        var connection = new FakeDbConnection { ScalarResult = 3 };
        var repository = new EntregableReadRepository(connection);

        var result = await repository.GetEntregablesByEjecucion(1m);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetAllAsync_ConnectionFails_ThrowsInvalidOperationException()
    {
        var connection = new FakeDbConnection { ThrowOnExecute = true };
        var repository = new EntregableReadRepository(connection);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetAllAsync());
    }
}