using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed class DeleteLogTransaccionCommandHandler : IRequestHandler<DeleteLogTransaccionCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteLogTransaccionCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteLogTransaccionCommand command, CancellationToken cancellationToken)
    {
        var logTransaccion = await _context.TblLogTransacciones
            .FirstOrDefaultAsync(lt => lt.IdTransaccion == command.IdTransaccion, cancellationToken);

        if (logTransaccion == null)
            throw new KeyNotFoundException($"Log de transacción con ID {command.IdTransaccion} no encontrado.");

        _context.TblLogTransacciones.Remove(logTransaccion);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}