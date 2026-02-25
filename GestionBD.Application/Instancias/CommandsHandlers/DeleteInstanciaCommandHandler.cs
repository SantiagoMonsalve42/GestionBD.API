using GestionBD.Application.Instancias.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Instancias.CommandsHandlers;

public sealed class DeleteInstanciaCommandHandler : IRequestHandler<DeleteInstanciaCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInstanciaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = await _unitOfWork.FindEntityAsync<TblInstancia>(command.IdInstancia, cancellationToken);

        if (instancia == null)
            throw new KeyNotFoundException($"Instancia con ID {command.IdInstancia} no encontrada.");

        _unitOfWork.Instancias.Remove(instancia);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}