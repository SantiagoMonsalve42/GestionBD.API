using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Motores.Commands;

public sealed class UpdateMotorCommandHandler : IRequestHandler<UpdateMotorCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateMotorCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateMotorCommand command, CancellationToken cancellationToken)
    {
        var motor = await _context.TblMotores
            .FirstOrDefaultAsync(m => m.IdMotor == command.Request.IdMotor, cancellationToken);

        if (motor == null)
            throw new KeyNotFoundException($"Motor con ID {command.Request.IdMotor} no encontrado.");

        motor.NombreMotor = command.Request.NombreMotor;
        motor.DescripcionMotor = command.Request.DescripcionMotor ?? string.Empty;
        motor.VersionMotor = command.Request.VersionMotor ?? string.Empty;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}