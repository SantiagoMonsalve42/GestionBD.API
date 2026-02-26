using GestionBD.Application.Configuration;
using GestionBD.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Services;

public sealed class SqlServerScriptExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ServerNameMissing_ThrowsInvalidOperationException()
    {
        var settings = Options.Create(new DacpacSettings
        {
            ServerName = null!,
            Username = "user",
            Password = "pass"
        });

        var executor = new SqlServerScriptExecutor(settings);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync("db", "SELECT 1"));
    }

    [Fact]
    public async Task ExecuteAsync_UsernameMissing_ThrowsInvalidOperationException()
    {
        var settings = Options.Create(new DacpacSettings
        {
            ServerName = "server",
            Username = null!,
            Password = "pass"
        });

        var executor = new SqlServerScriptExecutor(settings);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            executor.ExecuteAsync("db", "SELECT 1", "server"));
    }
}