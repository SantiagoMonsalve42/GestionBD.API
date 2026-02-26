using GestionBD.Application.Contracts.Ejecuciones;
using MediatR;

namespace GestionBD.Application.Ejecuciones.Queries;

public sealed record GetAllEjecucionesQuery : IRequest<IEnumerable<EjecucionResponse>>;