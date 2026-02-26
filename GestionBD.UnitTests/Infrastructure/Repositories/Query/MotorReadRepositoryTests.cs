using GestionBD.Infrastructure.Repositories.Query;
using GestionBD.UnitTests.Infrastructure.Repositories.Query.Fakes;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Repositories.Query;

public sealed class MotorReadRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_NoRows_ReturnsEmpty()
    {
        var connection = new FakeDbConnection();
        var repository = new MotorReadRepository(connection);

        var result = await repository.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ConnectionFails_ThrowsInvalidOperationException()
    {
        var connection = new FakeDbConnection { ThrowOnExecute = true };
        var repository = new MotorReadRepository(connection);

        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetAllAsync());
    }
}