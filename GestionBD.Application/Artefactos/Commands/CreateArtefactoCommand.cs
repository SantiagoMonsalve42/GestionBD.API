using MediatR;
using GestionBD.Application.Contracts.Artefactos;

namespace GestionBD.Application.Artefactos.Commands;

public sealed record CreateArtefactoCommand(CreateArtefactoRequest Request) : IRequest<decimal>;