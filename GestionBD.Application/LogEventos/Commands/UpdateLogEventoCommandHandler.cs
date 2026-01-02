using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.LogEventos.Commands;

public sealed class UpdateLogEventoCommandHandler : IRequestHandler<UpdateLogEventoCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateLogEventoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateLogEventoCommand command, CancellationToken cancellationToken)
    {
        var logEvento = await _context.TblLogEventos
            .FirstOrDefaultAsync(le => le.IdEvento == command.Request.IdEvento, cancellationToken);

        if (logEvento == null)
            throw new KeyNotFoundException($"Log de evento con ID {command.Request.IdEvento} no encontrado.");

        logEvento.IdTransaccion = command.Request.IdTransaccion;
        logEvento.FechaEjecucion = command.Request.FechaEjecucion;
        logEvento.Descripcion = command.Request.Descripcion;
        logEvento.EstadoEvento = command.Request.EstadoEvento;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}