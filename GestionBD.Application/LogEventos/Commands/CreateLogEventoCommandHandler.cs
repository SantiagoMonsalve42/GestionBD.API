using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.LogEventos.Commands;

public sealed class CreateLogEventoCommandHandler : IRequestHandler<CreateLogEventoCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateLogEventoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateLogEventoCommand command, CancellationToken cancellationToken)
    {
        var logEvento = new TblLogEvento
        {
            IdTransaccion = command.Request.IdTransaccion,
            FechaEjecucion = command.Request.FechaEjecucion,
            Descripcion = command.Request.Descripcion,
            EstadoEvento = command.Request.EstadoEvento
        };

        _context.TblLogEventos.Add(logEvento);
        await _context.SaveChangesAsync(cancellationToken);

        return logEvento.IdEvento;
    }
}