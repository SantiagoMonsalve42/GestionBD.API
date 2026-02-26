using GestionBD.Application.Entregables.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;
using MediatR;

namespace GestionBD.Application.Entregables.CommandsHandlers;

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
            IdEjecucion = command.Request.IdEjecucion,
            NumeroEntrega = command.Request.NumeroEntrega,
            IdEstado = (int)EstadoEntregaEnum.Creado
        };

        _unitOfWork.Entregables.Add(entregable);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entregable.IdEntregable;
    }
}