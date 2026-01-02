using MediatR;
using GestionBD.Application.Contracts.LogTransacciones;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed record UpdateLogTransaccionCommand(UpdateLogTransaccionRequest Request) : IRequest<Unit>;