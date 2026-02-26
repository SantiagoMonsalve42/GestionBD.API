using GestionBD.Application.Contracts.LogEventos;
using MediatR;

namespace GestionBD.Application.LogEventos.Commands;

public sealed record UpdateLogEventoCommand(UpdateLogEventoRequest Request) : IRequest<Unit>;