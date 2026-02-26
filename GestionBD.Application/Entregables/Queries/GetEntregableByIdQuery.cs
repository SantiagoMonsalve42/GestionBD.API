using GestionBD.Application.Contracts.Entregables;
using MediatR;

namespace GestionBD.Application.Entregables.Queries;

public sealed record GetEntregableByIdQuery(decimal IdEntregable) : IRequest<EntregableResponseEstado?>;