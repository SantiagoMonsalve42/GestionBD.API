using GestionBD.Application.Contracts.LogTransacciones;
using MediatR;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed record CreateLogTransaccionCommand(CreateLogTransaccionRequest Request) : IRequest<decimal>;