using GestionBD.Application.Artefactos.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Artefactos.CommandsHandlers;

public sealed class DeleteArtefactoCommandHandler : IRequestHandler<DeleteArtefactoCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArtefactoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteArtefactoCommand command, CancellationToken cancellationToken)
    {
        var artefacto = await _unitOfWork.FindEntityAsync<TblArtefacto>(command.IdArtefacto, cancellationToken);

        if (artefacto == null)
            throw new KeyNotFoundException($"Artefacto con ID {command.IdArtefacto} no encontrado.");

        _unitOfWork.Artefactos.Remove(artefacto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}