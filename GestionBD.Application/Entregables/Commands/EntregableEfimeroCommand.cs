using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record EntregableEfimeroCommand(decimal idEntregable) : IRequest<string>;
