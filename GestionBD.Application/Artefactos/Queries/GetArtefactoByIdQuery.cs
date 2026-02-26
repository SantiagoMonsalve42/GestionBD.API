using GestionBD.Application.Contracts.Artefactos;
using MediatR;

namespace GestionBD.Application.Artefactos.Queries;

public sealed record GetArtefactoByIdQuery(decimal IdArtefacto) : IRequest<ArtefactoResponse?>;