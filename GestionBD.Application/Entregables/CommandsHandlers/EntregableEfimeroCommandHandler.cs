using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public sealed class EntregableEfimeroCommandHandler : IRequestHandler<EntregableEfimeroCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInstanciaReadRepository _instanciaReadRepository;
        private readonly IDacpacService _dacpacService;

        public EntregableEfimeroCommandHandler(IUnitOfWork unitOfWork,
                                                IInstanciaReadRepository instanciaReadRepository,
                                                IDacpacService dacpacService)
        {
            _unitOfWork = unitOfWork;
            _instanciaReadRepository = instanciaReadRepository;
            _dacpacService = dacpacService;
        }

        public async Task<string> Handle(EntregableEfimeroCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var entregable = await _instanciaReadRepository.GetConnectionDetailsByEntregableIdAsync(request.idEntregable, cancellationToken);

                if (entregable == null)
                    throw new InvalidOperationException($"No se encontraron detalles de conexión para el entregable {request.idEntregable}");

                if (entregable.TemporalBD != null)
                    await _dacpacService.DropTemporaryDatabaseAsync(entregable.TemporalBD);


                var dacpacPath = await _dacpacService.ExtractDacpacAsync(
                    serverName: $"{entregable.Instancia},{entregable.Puerto}",
                    databaseName: entregable.NombreBD,
                    username: entregable.Usuario,
                    password: entregable.Password,
                    cancellationToken: cancellationToken
                );

                if (string.IsNullOrWhiteSpace(dacpacPath))
                    throw new InvalidOperationException($"No se pudo extraer el DACPAC para el entregable {request.idEntregable}");

                string? tempDatabaseName = null;
                tempDatabaseName = await _dacpacService.DeployDacpacToTemporaryDatabaseAsync(
                        dacpacPath: dacpacPath,
                        bdName: null,
                        cancellationToken: cancellationToken
                    );

                await _unitOfWork.Entregables.UpdateDACPAC(request.idEntregable, dacpacPath, tempDatabaseName, cancellationToken);
                await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Preparacion, cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
                return $"DACPAC creado en: {dacpacPath}. Base de datos temporal creada: {tempDatabaseName}";
                
            }catch(Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException("No se pudo iniciar la transacción.", ex);
            }
        }
    }
}
