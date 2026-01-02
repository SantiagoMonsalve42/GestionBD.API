using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record DeleteEntregableCommand(decimal IdEntregable) : IRequest<Unit>;