using GestionBD.Application.Contracts.Motores;
using MediatR;

namespace GestionBD.Application.Motores.Commands;

public sealed record UpdateMotorCommand(UpdateMotorRequest Request) : IRequest<Unit>;