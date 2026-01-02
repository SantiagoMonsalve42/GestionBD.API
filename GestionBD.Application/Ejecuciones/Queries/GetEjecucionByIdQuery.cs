using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Application.Ejecuciones.Queries;

public sealed record GetEjecucionByIdQuery(decimal IdEjecucion) : IRequest<EjecucionResponse?>;