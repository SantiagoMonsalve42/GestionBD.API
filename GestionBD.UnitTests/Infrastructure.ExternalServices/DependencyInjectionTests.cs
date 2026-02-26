using GestionBD.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionBD.UnitTests.Infrastructure.ExternalServices;

public sealed class DependencyInjectionTests
{
    [Fact]
    public async Task AddExternalServicesAsync_WithMissingVaultUri_ShouldThrowInvalidOperationException()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            services.AddExternalServicesAsync(configuration));

        Assert.Contains("Vault:Uri", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddExternalServicesAsync_WithMissingVaultToken_ShouldThrowInvalidOperationException()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Vault:Uri"] = "https://vault.local"
            })
            .Build();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            services.AddExternalServicesAsync(configuration));

        Assert.Contains("Vault:Token", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}