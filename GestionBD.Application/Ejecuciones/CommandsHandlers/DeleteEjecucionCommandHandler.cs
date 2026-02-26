using GestionBD.Application.Ejecuciones.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Ejecuciones.CommandsHandlers;

public sealed class DeleteEjecucionCommandHandler : IRequestHandler<DeleteEjecucionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEjecucionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteEjecucionCommand command, CancellationToken cancellationToken)
    {
        var ejecucion = await _unitOfWork.FindEntityAsync<TblEjecucione>(command.IdEjecucion, cancellationToken);

        if (ejecucion == null)
            throw new KeyNotFoundException($"Ejecución con ID {command.IdEjecucion} no encontrada.");

        _unitOfWork.Ejecuciones.Remove(ejecucion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}