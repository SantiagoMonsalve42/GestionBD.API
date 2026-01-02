using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed record UpdateEjecucionCommand(UpdateEjecucionRequest Request) : IRequest<Unit>;