using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Domain;
using MediatR;

namespace GestionBD.Application.Artefactos.CommandsHandlers;

public sealed class ChangeOrderArtefactoCommandHandler : IRequestHandler<ChangeOrderArtefactoCommand, bool>
{
    public readonly IUnitOfWork _unitOfWork;
    public ChangeOrderArtefactoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ChangeOrderArtefactoCommand request, CancellationToken cancellationToken)
    {
        var status = false;
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            Dictionary<decimal, int> keyValuePairs = new Dictionary<decimal, int>();
            foreach (var item in request.listado)
            {
                keyValuePairs.Add(item.IdArtefacto, item.OrdenEjecucion);
            }
            status = await _unitOfWork.Artefactos.UpdateOrder(keyValuePairs);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
        }
        return status;
    }
}
