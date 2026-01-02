using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.Motores.Commands;

public sealed class CreateMotorCommandHandler : IRequestHandler<CreateMotorCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateMotorCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateMotorCommand command, CancellationToken cancellationToken)
    {
        var motor = new TblMotore
        {
            NombreMotor = command.Request.NombreMotor,
            DescripcionMotor = command.Request.DescripcionMotor ?? string.Empty,
            VersionMotor = command.Request.VersionMotor ?? string.Empty
        };

        _context.TblMotores.Add(motor);
        await _context.SaveChangesAsync(cancellationToken);

        return motor.IdMotor;
    }
}