using GestionBD.Application.Contracts.Artefactos;
using MediatR;

namespace GestionBD.Application.Artefactos.Commands;

public sealed record ValidateArtefactoCommand(decimal idEntregable) : IRequest<IEnumerable<ValidateArtefactoResponse>>;
