using MediatR;

namespace GestionBD.Application.Entregables.Commands
{
    public sealed record GenerateRollbackCommand(decimal idEntregable) : IRequest<string?>;
}
