using GestionBD.Application.Contracts.LogTransacciones;
using MediatR;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed record UpdateLogTransaccionCommand(UpdateLogTransaccionRequest Request) : IRequest<Unit>;