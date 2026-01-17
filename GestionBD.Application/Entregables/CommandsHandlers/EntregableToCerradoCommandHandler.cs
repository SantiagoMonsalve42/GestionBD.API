using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public sealed class EntregableToCerradoCommandHandler: IRequestHandler<EntregableToCerradoCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EntregableToCerradoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(EntregableToCerradoCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Revision, cancellationToken);
            return Unit.Value;
        }
    }
}
