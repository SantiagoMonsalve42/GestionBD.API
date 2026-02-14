using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Services;
using GestionBD.Domain;
using GestionBD.Domain.Exceptions;
using MediatR;
using System.Collections.Generic;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public sealed class DesplegarEntregableCommandHandler :
        IRequestHandler<DesplegarEntregableCommand, string>
    {
        private readonly EntregableDeploymentService _deploymentService;
        private readonly IDeployLog _deployLog;
        private readonly IEntregableReadRepository _entregableReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DesplegarEntregableCommandHandler(EntregableDeploymentService deploymentService,
                                                 IDeployLog deployLog,
                                                 IEntregableReadRepository entregableReadRepository,
                                                 IUnitOfWork unitOfWork)
        {
            _deploymentService = deploymentService;
            _deployLog = deployLog;
            _entregableReadRepository = entregableReadRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Handle(DesplegarEntregableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable <EntregablePreValidateResponse> resultado = await _deploymentService.DeployAsync(request.idEntregable, cancellationToken);
                var entrable = await _entregableReadRepository.GetByIdAsync(request.idEntregable, cancellationToken);
                string logFileName = $"Despliegue_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string pathResult = await _deployLog.GenerarArchivoLog(resultado,entrable.RutaEntregable, logFileName);
                await _unitOfWork.Entregables.UpdateRutaResultado(request.idEntregable, pathResult);
                await _unitOfWork.CommitTransactionAsync();
                return $"Resultado del despliegue almacenado en la ruta {pathResult}.";
            }
            catch (InvalidOperationException ex)
            {
                throw new ValidationException("Entregable", ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                throw new ValidationException("Archivo", ex.Message);
            }
        }
    }
}


