using GestionBD.Application.Contracts.LogEventos;
using MediatR;

namespace GestionBD.Application.LogEventos.Queries;

public sealed record GetAllLogEventosQuery : IRequest<IEnumerable<LogEventoResponse>>;