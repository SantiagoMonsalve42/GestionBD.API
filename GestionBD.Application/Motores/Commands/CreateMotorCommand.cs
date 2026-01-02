using MediatR;
using GestionBD.Application.Contracts.Motores;

namespace GestionBD.Application.Motores.Commands;

public sealed record CreateMotorCommand(CreateMotorRequest Request) : IRequest<decimal>;