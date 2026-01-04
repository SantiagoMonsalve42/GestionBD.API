using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Entregables.Commands;

public sealed class CreateEntregableCommandHandler : IRequestHandler<CreateEntregableCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEntregableCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateEntregableCommand command, CancellationToken cancellationToken)
    {
        var entregable = new TblEntregable
        {
            RutaEntregable = command.Request.RutaEntregable,
            DescripcionEntregable = command.Request.DescripcionEntregable,
            IdEjecucion = command.Request.IdEjecucion
        };

        _unitOfWork.Entregables.Add(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entregable.IdEntregable;
    }
}