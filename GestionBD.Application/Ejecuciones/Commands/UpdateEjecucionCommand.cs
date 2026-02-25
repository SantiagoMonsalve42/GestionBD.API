using GestionBD.Application.Contracts.Ejecuciones;
using MediatR;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed record UpdateEjecucionCommand(UpdateEjecucionRequest Request) : IRequest<Unit>;