using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.LogEventos.Commands;

namespace GestionBD.Application.LogEventos.CommandsHandlers;

public sealed class DeleteLogEventoCommandHandler : IRequestHandler<DeleteLogEventoCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLogEventoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteLogEventoCommand command, CancellationToken cancellationToken)
    {
        var logEvento = await _unitOfWork.FindEntityAsync<TblLogEvento>(command.IdEvento, cancellationToken);

        if (logEvento == null)
            throw new KeyNotFoundException($"Log de evento con ID {command.IdEvento} no encontrado.");

        _unitOfWork.LogEventos.Remove(logEvento);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}