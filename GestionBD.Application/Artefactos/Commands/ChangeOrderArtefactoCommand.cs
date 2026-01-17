using GestionBD.Application.Contracts.Artefactos;
using MediatR;

namespace GestionBD.Application.Artefactos.Commands;

public sealed record ChangeOrderArtefactoCommand(List<ArtefactoChangeOrder> listado) : IRequest<bool>;
