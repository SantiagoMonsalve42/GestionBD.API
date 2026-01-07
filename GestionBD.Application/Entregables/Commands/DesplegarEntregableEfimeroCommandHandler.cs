using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Services;
using GestionBD.Domain.Exceptions;
using MediatR;

namespace GestionBD.Application.Entregables.Commands
{
    public sealed class DesplegarEntregableEfimeroCommandHandler 
        : IRequestHandler<DesplegarEntregableEfimeroCommand, IEnumerable<EntregablePreValidateResponse>>
    {
        private readonly EntregableDeploymentService _deploymentService;

        public DesplegarEntregableEfimeroCommandHandler(EntregableDeploymentService deploymentService)
        {
            _deploymentService = deploymentService;
        }

        public async Task<IEnumerable<EntregablePreValidateResponse>> Handle(
                    DesplegarEntregableEfimeroCommand request, 
                    CancellationToken cancellationToken)
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
