using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed class UpdateLogTransaccionCommandHandler : IRequestHandler<UpdateLogTransaccionCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateLogTransaccionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateLogTransaccionCommand command, CancellationToken cancellationToken)
    {
        var logTransaccion = await _context.TblLogTransacciones
            .FirstOrDefaultAsync(lt => lt.IdTransaccion == command.Request.IdTransaccion, cancellationToken);

        if (logTransaccion == null)
            throw new KeyNotFoundException($"Log de transacción con ID {command.Request.IdTransaccion} no encontrado.");

        logTransaccion.NombreTransaccion = command.Request.NombreTransaccion;
        logTransaccion.EstadoTransaccion = command.Request.EstadoTransaccion;
        logTransaccion.DescripcionTransaccion = command.Request.DescripcionTransaccion;
        logTransaccion.FechaInicio = command.Request.FechaInicio;
        logTransaccion.RespuestaTransaccion = command.Request.RespuestaTransaccion;
        logTransaccion.FechaFin = command.Request.FechaFin;
        logTransaccion.UsuarioEjecucion = command.Request.UsuarioEjecucion;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}