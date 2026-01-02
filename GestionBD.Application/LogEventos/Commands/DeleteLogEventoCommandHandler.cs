using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.LogEventos.Commands;

public sealed class DeleteLogEventoCommandHandler : IRequestHandler<DeleteLogEventoCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteLogEventoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteLogEventoCommand command, CancellationToken cancellationToken)
    {
        var logEvento = await _context.TblLogEventos
            .FirstOrDefaultAsync(le => le.IdEvento == command.IdEvento, cancellationToken);

        if (logEvento == null)
            throw new KeyNotFoundException($"Log de evento con ID {command.IdEvento} no encontrado.");

        _context.TblLogEventos.Remove(logEvento);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}