using MediatR;
using GestionBD.Application.Contracts.LogEventos;

namespace GestionBD.Application.LogEventos.Queries;

public sealed record GetAllLogEventosQuery : IRequest<IEnumerable<LogEventoResponse>>;