using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.LogTransacciones.Commands;

namespace GestionBD.Application.LogTransacciones.CommandsHandlers;

public sealed class DeleteLogTransaccionCommandHandler : IRequestHandler<DeleteLogTransaccionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLogTransaccionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteLogTransaccionCommand command, CancellationToken cancellationToken)
    {
        var logTransaccion = await _unitOfWork.FindEntityAsync<TblLogTransaccione>(command.IdTransaccion, cancellationToken);

        if (logTransaccion == null)
            throw new KeyNotFoundException($"Log de transacción con ID {command.IdTransaccion} no encontrado.");

        _unitOfWork.LogTransacciones.Remove(logTransaccion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}