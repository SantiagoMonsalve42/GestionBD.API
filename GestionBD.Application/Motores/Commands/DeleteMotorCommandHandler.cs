using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Motores.Commands;

public sealed class DeleteMotorCommandHandler : IRequestHandler<DeleteMotorCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public DeleteMotorCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteMotorCommand command, CancellationToken cancellationToken)
    {
        var motor = await _context.TblMotores
            .FirstOrDefaultAsync(m => m.IdMotor == command.IdMotor, cancellationToken);

        if (motor == null)
            throw new KeyNotFoundException($"Motor con ID {command.IdMotor} no encontrado.");

        _context.TblMotores.Remove(motor);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}