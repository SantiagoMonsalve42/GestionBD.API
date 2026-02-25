using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record DesplegarEntregableCommand(decimal idEntregable) : IRequest<string>;
