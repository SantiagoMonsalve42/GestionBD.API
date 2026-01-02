using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Entregables.Commands;

public sealed class UpdateEntregableCommandHandler : IRequestHandler<UpdateEntregableCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateEntregableCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = await _context.TblEntregables
            .FirstOrDefaultAsync(e => e.IdEntregable == command.Request.IdEntregable, cancellationToken);

        if (entregable == null)
            throw new KeyNotFoundException($"Entregable con ID {command.Request.IdEntregable} no encontrado.");

        entregable.RutaEntregable = command.Request.RutaEntregable;
        entregable.DescripcionEntregable = command.Request.DescripcionEntregable;
        entregable.IdEjecucion = command.Request.IdEjecucion;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}