using GestionBD.Application.Contracts.Artefactos;
using MediatR;

namespace GestionBD.Application.Artefactos.Queries;

public sealed record GetArtefactoByEntregableIdQuery(decimal IdEntregable) : IRequest<IEnumerable<ArtefactoResponse>>;
