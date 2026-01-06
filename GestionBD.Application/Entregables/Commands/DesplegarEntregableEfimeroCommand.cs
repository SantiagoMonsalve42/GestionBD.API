using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record DesplegarEntregableEfimeroCommand(decimal idEntregable) : IRequest<string>;
