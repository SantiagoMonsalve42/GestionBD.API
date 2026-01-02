using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Parametros.Commands;

public sealed class DeleteParametroCommandHandler : IRequestHandler<DeleteParametroCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteParametroCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteParametroCommand command, CancellationToken cancellationToken)
    {
        var parametro = await _context.TblParametros
            .FirstOrDefaultAsync(p => p.IdParametro == command.IdParametro, cancellationToken);

        if (parametro == null)
            throw new KeyNotFoundException($"Parámetro con ID {command.IdParametro} no encontrado.");

        _context.TblParametros.Remove(parametro);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}