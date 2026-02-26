using GestionBD.Application.Contracts.Entregables;
using MediatR;

namespace GestionBD.Application.Entregables.Queries;

public sealed record GetEntregablesRevisionQuery : IRequest<IEnumerable<EntregableRevisionResponse>>;