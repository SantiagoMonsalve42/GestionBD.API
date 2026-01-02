using MediatR;
using GestionBD.Application.Contracts.Motores;

namespace GestionBD.Application.Motores.Commands;

public sealed record UpdateMotorCommand(UpdateMotorRequest Request) : IRequest<Unit>;