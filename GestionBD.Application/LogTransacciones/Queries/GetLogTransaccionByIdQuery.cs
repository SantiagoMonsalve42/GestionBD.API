using GestionBD.Application.Contracts.LogTransacciones;
using MediatR;

namespace GestionBD.Application.LogTransacciones.Queries;

public sealed record GetLogTransaccionByIdQuery(decimal IdTransaccion) : IRequest<LogTransaccionResponse?>;