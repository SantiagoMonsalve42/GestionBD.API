using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.Entregables.Commands;

public sealed class CreateEntregableCommandHandler : IRequestHandler<CreateEntregableCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateEntregableCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = new TblEntregable
        {
            RutaEntregable = command.Request.RutaEntregable,
            DescripcionEntregable = command.Request.DescripcionEntregable,
            IdEjecucion = command.Request.IdEjecucion
        };

        _context.TblEntregables.Add(entregable);
        await _context.SaveChangesAsync(cancellationToken);

        return entregable.IdEntregable;
    }
}