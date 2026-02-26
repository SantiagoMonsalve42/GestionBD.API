using GestionBD.Infrastructure.Repositories.Query;
using GestionBD.UnitTests.Infrastructure.Repositories.Query.Fakes;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Query;

public sealed class ArtefactoReadRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_NoRows_ReturnsNull()
    {
        var connection = new FakeDbConnection();
        var repository = new ArtefactoReadRepository(connection);

        var result = await repository.GetByIdAsync(1m);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ConnectionFails_ThrowsInvalidOperationException()
    {
        var connection = new FakeDbConnection { ThrowOnExecute = true };
        var repository = new ArtefactoReadRepository(connection);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetAllAsync());
    }
}