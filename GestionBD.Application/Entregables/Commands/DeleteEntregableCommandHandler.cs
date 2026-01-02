using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Entregables.Commands;

public sealed class DeleteEntregableCommandHandler : IRequestHandler<DeleteEntregableCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteEntregableCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = await _context.TblEntregables
            .FirstOrDefaultAsync(e => e.IdEntregable == command.IdEntregable, cancellationToken);

        if (entregable == null)
            throw new KeyNotFoundException($"Entregable con ID {command.IdEntregable} no encontrado.");

        _context.TblEntregables.Remove(entregable);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}