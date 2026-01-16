using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public sealed class EntregableToRevisionCommandHandler : IRequestHandler<EntregableToRevisionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EntregableToRevisionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        public async Task<Unit> Handle(EntregableToRevisionCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Revision, cancellationToken);
            return Unit.Value;
        }
    }
}
