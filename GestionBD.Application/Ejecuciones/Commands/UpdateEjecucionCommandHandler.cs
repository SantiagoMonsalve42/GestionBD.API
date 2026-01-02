using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed class UpdateEjecucionCommandHandler : IRequestHandler<UpdateEjecucionCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateEjecucionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateEjecucionCommand command, CancellationToken cancellationToken)
    {
        var ejecucion = await _context.TblEjecuciones
            .FirstOrDefaultAsync(e => e.IdEjecucion == command.Request.IdEjecucion, cancellationToken);

        if (ejecucion == null)
            throw new KeyNotFoundException($"Ejecución con ID {command.Request.IdEjecucion} no encontrada.");

        ejecucion.IdInstancia = command.Request.IdInstancia;
        ejecucion.HoraInicioEjecucion = command.Request.HoraInicioEjecucion;
        ejecucion.HoraFinEjecucion = command.Request.HoraFinEjecucion;
        ejecucion.Descripcion = command.Request.Descripcion;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}