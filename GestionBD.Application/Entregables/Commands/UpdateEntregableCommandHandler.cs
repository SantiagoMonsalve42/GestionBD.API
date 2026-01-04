using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Entregables.Commands;

public sealed class UpdateEntregableCommandHandler : IRequestHandler<UpdateEntregableCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEntregableCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = await _unitOfWork.FindEntityAsync<TblEntregable>(command.Request.IdEntregable, cancellationToken);

        if (entregable == null)
            throw new KeyNotFoundException($"Entregable con ID {command.Request.IdEntregable} no encontrado.");

        entregable.RutaEntregable = command.Request.RutaEntregable;
        entregable.DescripcionEntregable = command.Request.DescripcionEntregable;
        entregable.IdEjecucion = command.Request.IdEjecucion;

        _unitOfWork.Entregables.Update(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}