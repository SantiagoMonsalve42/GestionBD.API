using GestionBD.Application.Contracts.Motores;
using MediatR;

namespace GestionBD.Application.Motores.Queries;

public sealed record GetMotorByIdQuery(decimal IdMotor) : IRequest<MotorResponse?>;