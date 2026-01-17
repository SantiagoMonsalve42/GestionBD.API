using MediatR;
using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Entregables.Commands;

public sealed record CreateEntregableCommand(CreateEntregableRequest Request) : IRequest<decimal>;