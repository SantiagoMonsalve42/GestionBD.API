using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.LogTransacciones.Commands;

namespace GestionBD.Application.LogTransacciones.CommandsHandlers;

public sealed class CreateLogTransaccionCommandHandler : IRequestHandler<CreateLogTransaccionCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateLogTransaccionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateLogTransaccionCommand command, CancellationToken cancellationToken)
    {
        var logTransaccion = new TblLogTransaccione
        {
            NombreTransaccion = command.Request.NombreTransaccion,
            EstadoTransaccion = command.Request.EstadoTransaccion,
            DescripcionTransaccion = command.Request.DescripcionTransaccion,
            FechaInicio = command.Request.FechaInicio,
            RespuestaTransaccion = command.Request.RespuestaTransaccion,
            FechaFin = command.Request.FechaFin,
            UsuarioEjecucion = command.Request.UsuarioEjecucion
        };

        _unitOfWork.LogTransacciones.Add(logTransaccion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return logTransaccion.IdTransaccion;
    }
}