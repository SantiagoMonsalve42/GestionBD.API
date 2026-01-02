using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Artefactos.Commands;

public sealed class DeleteArtefactoCommandHandler : IRequestHandler<DeleteArtefactoCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteArtefactoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteArtefactoCommand command, CancellationToken cancellationToken)
    {
        var artefacto = await _context.TblArtefactos
            .FirstOrDefaultAsync(a => a.IdArtefacto == command.IdArtefacto, cancellationToken);

        if (artefacto == null)
            throw new KeyNotFoundException($"Artefacto con ID {command.IdArtefacto} no encontrado.");

        _context.TblArtefactos.Remove(artefacto);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}