using GestionBD.Application.Contracts.Entregables;
using MediatR;

namespace GestionBD.Application.Entregables.Queries;

public sealed record GetAllEntregablesByEjecucionIdQuery(decimal IdEjecucion) : IRequest<IEnumerable<EntregableResponseEstado>>;