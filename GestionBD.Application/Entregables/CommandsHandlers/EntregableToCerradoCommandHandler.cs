using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers
{
    public sealed class EntregableToCerradoCommandHandler: IRequestHandler<EntregableToCerradoCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntregableReadRepository _entregableReadRepository;
        private readonly IDacpacService _dacpacService;
        public EntregableToCerradoCommandHandler(IUnitOfWork unitOfWork,
                                                IEntregableReadRepository entregableReadRepository,
                                                IDacpacService dacpacService)
        {
            _unitOfWork = unitOfWork;
            _entregableReadRepository = entregableReadRepository;
            _dacpacService = dacpacService;
        }
        public async Task<Unit> Handle(EntregableToCerradoCommand request, CancellationToken cancellationToken)
        {
            var entregable = await _entregableReadRepository.GetByIdAsync(request.idEntregable);
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                if (entregable == null)
                {
                    throw new NullReferenceException("No existe entregable con ese id");
                }
                if (request.close == 1)
                {
                    await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Cerrado, cancellationToken);
                }
                else
                {
                    await _unitOfWork.Entregables.UpdateEstado(request.idEntregable, Domain.Enum.EstadoEntregaEnum.Creado, cancellationToken);
                }

                if (!string.IsNullOrEmpty(entregable?.TemporalBD))
                {
                    await _dacpacService.DropTemporaryDatabaseAsync(entregable.TemporalBD);
                    await _unitOfWork.Entregables.removeDatabase(request.idEntregable, cancellationToken);
                }
                await _unitOfWork.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Error al actualizar el estado del entregable: {ex.Message}");
            }
            
            return Unit.Value;
        }
    }
}
