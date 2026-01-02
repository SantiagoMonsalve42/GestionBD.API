using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed record CreateEjecucionCommand(CreateEjecucionRequest Request) : IRequest<decimal>;