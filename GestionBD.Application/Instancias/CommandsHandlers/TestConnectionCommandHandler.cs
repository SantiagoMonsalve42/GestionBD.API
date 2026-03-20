using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Instancias;
using MediatR;

namespace GestionBD.Application.Instancias.CommandsHandlers
{
    public class TestConnectionCommandHandler : IRequestHandler<TestConnectionCommand, bool>
    {
        private readonly IInstanciaReadRepository _instanciaReadRepository;
        private readonly IDatabaseService _databaseService;
        private readonly IVaultConfigurationProvider _vaultConfigurationProvider;
        public TestConnectionCommandHandler(IInstanciaReadRepository instanciaReadRepository,
                                            IDatabaseService databaseService,
                                            IVaultConfigurationProvider vaultConfigurationProvider)
        {
            _instanciaReadRepository = instanciaReadRepository;
            _databaseService = databaseService;
            _vaultConfigurationProvider = vaultConfigurationProvider;
        }
        public async Task<bool> Handle(TestConnectionCommand request, CancellationToken cancellationToken)
        {
            var instanciaConnect = await _instanciaReadRepository.GetByIdAsync(request.IdInstancia, cancellationToken);
            var vaultPath = await _vaultConfigurationProvider.GetSecretsAsync(instanciaConnect.SessionPath);
            bool connectionSuccessful = await _databaseService.testConnection(
                $"{instanciaConnect.Instancia},{instanciaConnect.Puerto}",
                instanciaConnect.NombreBD,
                vaultPath["user"].ToString() ?? "",
                vaultPath["pass"].ToString() ?? "");
            return connectionSuccessful;
        }
    }
}
