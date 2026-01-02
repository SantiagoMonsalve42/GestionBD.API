using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed class CreateEjecucionCommandHandler : IRequestHandler<CreateEjecucionCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateEjecucionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateEjecucionCommand command, CancellationToken cancellationToken)
    {
        var ejecucion = new TblEjecucione
        {
            IdInstancia = command.Request.IdInstancia,
            HoraInicioEjecucion = command.Request.HoraInicioEjecucion,
            HoraFinEjecucion = command.Request.HoraFinEjecucion,
            Descripcion = command.Request.Descripcion
        };

        _context.TblEjecuciones.Add(ejecucion);
        await _context.SaveChangesAsync(cancellationToken);

        return ejecucion.IdEjecucion;
    }
}