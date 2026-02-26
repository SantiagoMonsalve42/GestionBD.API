using GestionBD.Infrastructure.Repositories.Query;
using GestionBD.UnitTests.Infrastructure.Repositories.Query.Fakes;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Query;

public sealed class EjecucionReadRepositoryTests
{
    [Fact]
    public async Task ExistsByReqName_ScalarResult_ReturnsValue()
    {
        var connection = new FakeDbConnection { ScalarResult = true };
        var repository = new EjecucionReadRepository(connection);

        var result = await repository.ExistsByReqName("REQ-1");

        Assert.True(result);
    }

    [Fact]
    public async Task GetAllAsync_ConnectionFails_ThrowsInvalidOperationException()
    {
        var connection = new FakeDbConnection { ThrowOnExecute = true };
        var repository = new EjecucionReadRepository(connection);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetAllAsync());
    }
}