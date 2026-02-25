using GestionBD.Infrastructure.Services;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Services;

public sealed class DatabaseServiceTests
{
    [Fact]
    public async Task GetObjectDefinition_EmptyServer_ThrowsArgumentException()
    {
        var service = new DatabaseService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.getObjectDefinition("", "db", "user", "pass", "dbo.Table"));
    }

    [Fact]
    public async Task GetObjectDefinition_EmptyDatabase_ThrowsArgumentException()
    {
        var service = new DatabaseService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.getObjectDefinition("server", "", "user", "pass", "dbo.Table"));
    }

    [Fact]
    public async Task GetObjectDefinition_EmptyObjectName_ThrowsArgumentException()
    {
        var service = new DatabaseService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.getObjectDefinition("server", "db", "user", "pass", ""));
    }
}