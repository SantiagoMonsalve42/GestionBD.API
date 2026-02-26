using GestionBD.Application.Contracts.Motores;
using MediatR;

namespace GestionBD.Application.Motores.Queries;

public sealed record GetAllMotoresQuery : IRequest<IEnumerable<MotorResponse>>;