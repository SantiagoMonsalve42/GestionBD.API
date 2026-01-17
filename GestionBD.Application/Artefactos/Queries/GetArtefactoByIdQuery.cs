using MediatR;
using GestionBD.Application.Contracts.Artefactos;

namespace GestionBD.Application.Artefactos.Queries;

public sealed record GetArtefactoByIdQuery(decimal IdArtefacto) : IRequest<ArtefactoResponse?>;