using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.Motores.Commands;

namespace GestionBD.Application.Motores.CommandsHandlers;

public sealed class UpdateMotorCommandHandler : IRequestHandler<UpdateMotorCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMotorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMotorCommand command, CancellationToken cancellationToken)
    {
        var motor = await _unitOfWork.FindEntityAsync<TblMotore>(command.Request.IdMotor, cancellationToken);

        if (motor == null)
            throw new KeyNotFoundException($"Motor con ID {command.Request.IdMotor} no encontrado.");

        motor.NombreMotor = command.Request.NombreMotor;
        motor.DescripcionMotor = command.Request.DescripcionMotor ?? string.Empty;
        motor.VersionMotor = command.Request.VersionMotor ?? string.Empty;

        _unitOfWork.Motores.Update(motor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}