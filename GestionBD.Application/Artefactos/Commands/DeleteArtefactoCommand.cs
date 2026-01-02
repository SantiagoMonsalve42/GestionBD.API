using MediatR;

namespace GestionBD.Application.Artefactos.Commands;

public sealed record DeleteArtefactoCommand(decimal IdArtefacto) : IRequest<Unit>;