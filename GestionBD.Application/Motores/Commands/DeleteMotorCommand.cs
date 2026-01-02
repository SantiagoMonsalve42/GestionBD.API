using MediatR;

namespace GestionBD.Application.Motores.Commands;

public sealed record DeleteMotorCommand(decimal IdMotor) : IRequest<Unit>;