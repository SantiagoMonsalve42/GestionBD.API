using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record EntregableToRevisionCommand(decimal idEntregable) : IRequest<Unit>;
