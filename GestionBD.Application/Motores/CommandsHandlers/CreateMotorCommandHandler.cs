using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.Motores.Commands;

namespace GestionBD.Application.Motores.CommandsHandlers;

public sealed class CreateMotorCommandHandler : IRequestHandler<CreateMotorCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMotorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateMotorCommand command, CancellationToken cancellationToken)
    {
        var motor = new TblMotore
        {
            NombreMotor = command.Request.NombreMotor,
            DescripcionMotor = command.Request.DescripcionMotor ?? string.Empty,
            VersionMotor = command.Request.VersionMotor ?? string.Empty
        };

        _unitOfWork.Motores.Add(motor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return motor.IdMotor;
    }
}