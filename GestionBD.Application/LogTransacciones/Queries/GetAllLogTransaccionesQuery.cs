using GestionBD.Application.Contracts.LogTransacciones;
using MediatR;

namespace GestionBD.Application.LogTransacciones.Queries;

public sealed record GetAllLogTransaccionesQuery : IRequest<IEnumerable<LogTransaccionResponse>>;