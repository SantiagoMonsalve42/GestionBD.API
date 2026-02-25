using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Configuration;
using GestionBD.Infrastructure.ExternalServices.Vault;

namespace GestionBD.UnitTests.Infrastructure.ExternalServices.Vault;

public sealed class VaultConfigurationLoaderTests
{
    [Fact]
    public async Task LoadSettings_ShouldUseExpectedPathsAndReturnValues()
    {
        var dacpacSettings = new DacpacSettings { ServerName = "server" };
        var fileStorageSettings = new FileStorageSettings { BasePath = "/tmp" };
        var connectionStringsSettings = new ConnectionStringsSettings { DefaultConnection = "conn" };
        var openIASettings = new OpenIASettings { Model = "gpt-4", ApiKey = "key", BaseURL = "https://openai" };
        var keycloakSettings = new KeycloakSettings { Authority = "auth" };

        var provider = new FakeVaultConfigurationProvider(new Dictionary<string, object>
        {
            ["gestionbd/DacpacSettings"] = dacpacSettings,
            ["gestionbd/FileStorage"] = fileStorageSettings,
            ["gestionbd/ConnectionStrings"] = connectionStringsSettings,
            ["gestionbd/OpenIASettings"] = openIASettings,
            ["gestionbd/KeycloakSettings"] = keycloakSettings
        });

        var loader = new VaultConfigurationLoader(provider);

        var dacpacResult = await loader.LoadDacpacSettingsAsync();
        var fileStorageResult = await loader.LoadFileStorageSettingsAsync();
        var connectionStringsResult = await loader.LoadConnectionStringsAsync();
        var openIAResult = await loader.LoadOpenIASettingsAsync();
        var keycloakResult = await loader.LoadKeycloakSettingsAsync();

        Assert.Same(dacpacSettings, dacpacResult);
        Assert.Same(fileStorageSettings, fileStorageResult);
        Assert.Same(connectionStringsSettings, connectionStringsResult);
        Assert.Same(openIASettings, openIAResult);
        Assert.Same(keycloakSettings, keycloakResult);
    }

    private sealed class FakeVaultConfigurationProvider : IVaultConfigurationProvider
    {
        private readonly IDictionary<string, object> _storage;

        public FakeVaultConfigurationProvider(IDictionary<string, object> storage)
        {
            _storage = storage;
        }

        public Task<T> GetConfigurationAsync<T>(string path, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            if (_storage.TryGetValue(path, out var value))
            {
                return Task.FromResult((T)value);
            }

            return Task.FromResult(new T());
        }

        public Task<IDictionary<string, object>> GetSecretsAsync(
            string path,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>());
        }
    }
}