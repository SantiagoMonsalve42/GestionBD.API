using GestionBD.Application.LogEventos.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.LogEventos.CommandsHandlers;

public sealed class UpdateLogEventoCommandHandler : IRequestHandler<UpdateLogEventoCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLogEventoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateLogEventoCommand command, CancellationToken cancellationToken)
    {
        var logEvento = await _unitOfWork.FindEntityAsync<TblLogEvento>(command.Request.IdEvento, cancellationToken);

        if (logEvento == null)
            throw new KeyNotFoundException($"Log de evento con ID {command.Request.IdEvento} no encontrado.");

        logEvento.IdTransaccion = command.Request.IdTransaccion;
        logEvento.FechaEjecucion = command.Request.FechaEjecucion;
        logEvento.Descripcion = command.Request.Descripcion;
        logEvento.EstadoEvento = command.Request.EstadoEvento;

        _unitOfWork.LogEventos.Update(logEvento);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}