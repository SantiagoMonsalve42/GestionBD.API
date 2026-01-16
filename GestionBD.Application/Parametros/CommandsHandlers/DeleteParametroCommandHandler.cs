using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.Parametros.Commands;

namespace GestionBD.Application.Parametros.CommandsHandlers;

public sealed class DeleteParametroCommandHandler : IRequestHandler<DeleteParametroCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteParametroCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteParametroCommand command, CancellationToken cancellationToken)
    {
        var parametro = await _unitOfWork.FindEntityAsync<TblParametro>(command.IdParametro, cancellationToken);

        if (parametro == null)
            throw new KeyNotFoundException($"Parámetro con ID {command.IdParametro} no encontrado.");

        _unitOfWork.Parametros.Remove(parametro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}