using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed class CreateLogTransaccionCommandHandler : IRequestHandler<CreateLogTransaccionCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateLogTransaccionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateLogTransaccionCommand command, CancellationToken cancellationToken)
    {
        var logTransaccion = new TblLogTransaccione
        {
            NombreTransaccion = command.Request.NombreTransaccion,
            EstadoTransaccion = command.Request.EstadoTransaccion,
            DescripcionTransaccion = command.Request.DescripcionTransaccion,
            FechaInicio = command.Request.FechaInicio,
            RespuestaTransaccion = command.Request.RespuestaTransaccion,
            FechaFin = command.Request.FechaFin,
            UsuarioEjecucion = command.Request.UsuarioEjecucion
        };

        _context.TblLogTransacciones.Add(logTransaccion);
        await _context.SaveChangesAsync(cancellationToken);

        return logTransaccion.IdTransaccion;
    }
}