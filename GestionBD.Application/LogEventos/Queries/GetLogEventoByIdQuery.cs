using GestionBD.Application.Contracts.LogEventos;
using MediatR;

namespace GestionBD.Application.LogEventos.Queries;

public sealed record GetLogEventoByIdQuery(decimal IdEvento) : IRequest<LogEventoResponse?>;