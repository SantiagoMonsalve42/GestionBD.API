using MediatR;
using GestionBD.Application.Contracts.LogTransacciones;

namespace GestionBD.Application.LogTransacciones.Queries;

public sealed record GetAllLogTransaccionesQuery : IRequest<IEnumerable<LogTransaccionResponse>>;