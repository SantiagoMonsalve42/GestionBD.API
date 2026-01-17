using MediatR;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed record DeleteLogTransaccionCommand(decimal IdTransaccion) : IRequest<Unit>;