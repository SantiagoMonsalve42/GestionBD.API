using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Entregables.Commands;

public sealed class DeleteEntregableCommandHandler : IRequestHandler<DeleteEntregableCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEntregableCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = await _unitOfWork.FindEntityAsync<TblEntregable>(command.IdEntregable, cancellationToken);

        if (entregable == null)
            throw new KeyNotFoundException($"Entregable con ID {command.IdEntregable} no encontrado.");

        _unitOfWork.Entregables.Remove(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}