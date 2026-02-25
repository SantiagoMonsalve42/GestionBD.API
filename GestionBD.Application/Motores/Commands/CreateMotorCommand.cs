using GestionBD.Application.Contracts.Motores;
using MediatR;

namespace GestionBD.Application.Motores.Commands;

public sealed record CreateMotorCommand(CreateMotorRequest Request) : IRequest<decimal>;