using MediatR;
using GestionBD.Application.Contracts.Motores;

namespace GestionBD.Application.Motores.Queries;

public sealed record GetAllMotoresQuery : IRequest<IEnumerable<MotorResponse>>;