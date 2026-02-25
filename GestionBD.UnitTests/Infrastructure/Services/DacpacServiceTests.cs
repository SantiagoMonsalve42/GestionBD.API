using GestionBD.Application.Configuration;
using GestionBD.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Services;

public sealed class DacpacServiceTests
{
    [Fact]
    public void Create_WithMissingBasePath_ThrowsInvalidOperationException()
    {
        var dacpacSettings = Options.Create(new DacpacSettings
        {
            ServerName = "server",
            Username = "user",
            Password = "pass"
        });

        var storageSettings = Options.Create(new FileStorageSettings
        {
            BasePathDACPAC = null!,
            BasePath = string.Empty,
            BasePathPrompts = string.Empty
        });

        Assert.Throws<InvalidOperationException>(() =>
            new DacpacService(dacpacSettings, storageSettings));
    }

    [Fact]
    public async Task ExtractDacpacAsync_EmptyServer_ThrowsArgumentException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ExtractDacpacAsync("", "db"));
    }

    [Fact]
    public async Task DeployDacpacToTemporaryDatabaseAsync_EmptyPath_ThrowsArgumentException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.DeployDacpacToTemporaryDatabaseAsync(""));
    }

    [Fact]
    public async Task DropTemporaryDatabaseAsync_InvalidName_ThrowsInvalidOperationException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DropTemporaryDatabaseAsync("MainDb"));
    }

    private static DacpacService CreateService()
    {
        var dacpacSettings = Options.Create(new DacpacSettings
        {
            ServerName = "server",
            Username = "user",
            Password = "pass"
        });

        var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var storageSettings = Options.Create(new FileStorageSettings
        {
            BasePathDACPAC = basePath,
            BasePath = string.Empty,
            BasePathPrompts = string.Empty
        });

        return new DacpacService(dacpacSettings, storageSettings);
    }
}