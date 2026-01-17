using MediatR;
using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Entregables.Commands;

public sealed record CreateEntregableWithFileCommand(
    CreateEntregableWithFileRequest Request
) : IRequest<decimal>;