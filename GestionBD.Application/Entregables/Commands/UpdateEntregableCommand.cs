using MediatR;
using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Entregables.Commands;

public sealed record UpdateEntregableCommand(UpdateEntregableRequest Request) : IRequest<Unit>;