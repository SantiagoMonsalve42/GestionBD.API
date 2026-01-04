using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed class UpdateEjecucionCommandHandler : IRequestHandler<UpdateEjecucionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEjecucionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateEjecucionCommand command, CancellationToken cancellationToken)
    {
        var ejecucion = await _unitOfWork.FindEntityAsync<TblEjecucione>(command.Request.IdEjecucion, cancellationToken);

        if (ejecucion == null)
            throw new KeyNotFoundException($"Ejecución con ID {command.Request.IdEjecucion} no encontrada.");

        ejecucion.IdInstancia = command.Request.IdInstancia;
        ejecucion.HoraInicioEjecucion = command.Request.HoraInicioEjecucion;
        ejecucion.HoraFinEjecucion = command.Request.HoraFinEjecucion;
        ejecucion.Descripcion = command.Request.Descripcion;
        ejecucion.NombreRequerimiento = command.Request.NombreRequerimiento;

        _unitOfWork.Ejecuciones.Update(ejecucion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}