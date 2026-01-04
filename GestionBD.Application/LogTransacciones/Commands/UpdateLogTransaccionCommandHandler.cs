using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.LogTransacciones.Commands;

public sealed class UpdateLogTransaccionCommandHandler : IRequestHandler<UpdateLogTransaccionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLogTransaccionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateLogTransaccionCommand command, CancellationToken cancellationToken)
    {
        var logTransaccion = await _unitOfWork.FindEntityAsync<TblLogTransaccione>(command.Request.IdTransaccion, cancellationToken);

        if (logTransaccion == null)
            throw new KeyNotFoundException($"Log de transacción con ID {command.Request.IdTransaccion} no encontrado.");

        logTransaccion.NombreTransaccion = command.Request.NombreTransaccion;
        logTransaccion.EstadoTransaccion = command.Request.EstadoTransaccion;
        logTransaccion.DescripcionTransaccion = command.Request.DescripcionTransaccion;
        logTransaccion.FechaInicio = command.Request.FechaInicio;
        logTransaccion.RespuestaTransaccion = command.Request.RespuestaTransaccion;
        logTransaccion.FechaFin = command.Request.FechaFin;
        logTransaccion.UsuarioEjecucion = command.Request.UsuarioEjecucion;

        _unitOfWork.LogTransacciones.Update(logTransaccion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}