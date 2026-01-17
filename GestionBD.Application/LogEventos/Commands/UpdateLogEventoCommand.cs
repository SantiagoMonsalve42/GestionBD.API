using MediatR;
using GestionBD.Application.Contracts.LogEventos;

namespace GestionBD.Application.LogEventos.Commands;

public sealed record UpdateLogEventoCommand(UpdateLogEventoRequest Request) : IRequest<Unit>;