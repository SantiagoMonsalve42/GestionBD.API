using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Services;
using GestionBD.Domain.Exceptions;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public sealed class DesplegarEntregableCommandHandler :
        IRequestHandler<DesplegarEntregableCommand, IEnumerable<EntregablePreValidateResponse>>
    {
        private readonly EntregableDeploymentService _deploymentService;

        public DesplegarEntregableCommandHandler(EntregableDeploymentService deploymentService)
        {
            _deploymentService = deploymentService;
        }
        public async Task<IEnumerable<EntregablePreValidateResponse>> Handle(DesplegarEntregableCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _deploymentService.DeployAsync(request.idEntregable, cancellationToken);
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


