using GestionBD.Infrastructure.ExternalServices.Vault;

namespace GestionBD.UnitTests.Infrastructure.ExternalServices.Vault;

public sealed class HashiCorpVaultConfigurationProviderTests
{
    [Fact]
    public void Constructor_WithEmptyVaultUri_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new HashiCorpVaultConfigurationProvider(string.Empty, "token"));

        Assert.Equal("vaultUri", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithEmptyToken_ShouldThrowArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new HashiCorpVaultConfigurationProvider("https://vault.local", string.Empty));

        Assert.Equal("token", exception.ParamName);
    }

    [Fact]
    public async Task GetSecretsAsync_WithEmptyPath_ShouldThrowArgumentException()
    {
        var provider = new HashiCorpVaultConfigurationProvider("https://vault.local", "token");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            provider.GetSecretsAsync(string.Empty));

        Assert.Equal("path", exception.ParamName);
    }

    [Fact]
    public async Task GetConfigurationAsync_WithEmptyPath_ShouldThrowArgumentException()
    {
        var provider = new HashiCorpVaultConfigurationProvider("https://vault.local", "token");

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            provider.GetConfigurationAsync<object>(string.Empty));

        Assert.Equal("path", exception.ParamName);
    }
}