using MediatR;
using GestionBD.Application.Contracts.Motores;

namespace GestionBD.Application.Motores.Queries;

public sealed record GetMotorByIdQuery(decimal IdMotor) : IRequest<MotorResponse?>;