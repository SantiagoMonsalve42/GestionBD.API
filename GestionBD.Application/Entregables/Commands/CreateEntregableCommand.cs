using GestionBD.Application.Contracts.Entregables;
using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed record CreateEntregableCommand(CreateEntregableRequest Request) : IRequest<decimal>;