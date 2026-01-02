using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed class DeleteEjecucionCommandHandler : IRequestHandler<DeleteEjecucionCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteEjecucionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteEjecucionCommand command, CancellationToken cancellationToken)
    {
        var ejecucion = await _context.TblEjecuciones
            .FirstOrDefaultAsync(e => e.IdEjecucion == command.IdEjecucion, cancellationToken);

        if (ejecucion == null)
            throw new KeyNotFoundException($"Ejecución con ID {command.IdEjecucion} no encontrada.");

        _context.TblEjecuciones.Remove(ejecucion);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}