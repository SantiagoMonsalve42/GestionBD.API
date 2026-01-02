using MediatR;
using GestionBD.Application.Contracts.Artefactos;

namespace GestionBD.Application.Artefactos.Queries;

public sealed record GetAllArtefactosQuery : IRequest<IEnumerable<ArtefactoResponse>>;