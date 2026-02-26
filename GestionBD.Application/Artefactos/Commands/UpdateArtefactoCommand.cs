using GestionBD.Application.Contracts.Artefactos;
using MediatR;

namespace GestionBD.Application.Artefactos.Commands;

public sealed record UpdateArtefactoCommand(UpdateArtefactoRequest Request) : IRequest<Unit>;