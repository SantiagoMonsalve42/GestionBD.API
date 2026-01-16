using MediatR;

namespace GestionBD.Application.Entregables.Commands;


public sealed record EntregableToCerradoCommand(decimal idEntregable) : IRequest<Unit>;