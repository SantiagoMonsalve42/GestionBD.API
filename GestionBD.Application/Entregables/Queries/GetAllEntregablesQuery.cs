using MediatR;
using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Entregables.Queries;

public sealed record GetAllEntregablesQuery : IRequest<IEnumerable<EntregableResponse>>;