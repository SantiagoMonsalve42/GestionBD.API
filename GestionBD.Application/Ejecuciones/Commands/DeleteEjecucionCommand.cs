using MediatR;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed record DeleteEjecucionCommand(decimal IdEjecucion) : IRequest<Unit>;