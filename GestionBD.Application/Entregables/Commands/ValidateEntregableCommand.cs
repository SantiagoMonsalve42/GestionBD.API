using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record ValidateEntregableCommand(decimal idEntregable) : IRequest<string>;
