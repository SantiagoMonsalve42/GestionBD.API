using MediatR;

namespace GestionBD.Application.LogEventos.Commands;

public sealed record DeleteLogEventoCommand(decimal IdEvento) : IRequest<Unit>;