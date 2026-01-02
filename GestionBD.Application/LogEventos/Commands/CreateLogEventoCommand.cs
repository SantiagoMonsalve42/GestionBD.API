using MediatR;
using GestionBD.Application.Contracts.LogEventos;

namespace GestionBD.Application.LogEventos.Commands;

public sealed record CreateLogEventoCommand(CreateLogEventoRequest Request) : IRequest<decimal>;