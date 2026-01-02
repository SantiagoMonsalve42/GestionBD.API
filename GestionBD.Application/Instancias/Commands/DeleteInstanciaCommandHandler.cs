using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Instancias.Commands;

public sealed class DeleteInstanciaCommandHandler : IRequestHandler<DeleteInstanciaCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteInstanciaCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = await _context.TblInstancias
            .FirstOrDefaultAsync(i => i.IdInstancia == command.IdInstancia, cancellationToken);

        if (instancia == null)
            throw new KeyNotFoundException($"Instancia con ID {command.IdInstancia} no encontrada.");

        _context.TblInstancias.Remove(instancia);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}