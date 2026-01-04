using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Motores.Commands;

public sealed class DeleteMotorCommandHandler : IRequestHandler<DeleteMotorCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMotorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteMotorCommand command, CancellationToken cancellationToken)
    {
        var motor = await _unitOfWork.FindEntityAsync<TblMotore>(command.IdMotor, cancellationToken);

        if (motor == null)
            throw new KeyNotFoundException($"Motor con ID {command.IdMotor} no encontrado.");

        _unitOfWork.Motores.Remove(motor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}