using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed class CreateEjecucionCommandHandler : IRequestHandler<CreateEjecucionCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEjecucionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateEjecucionCommand command, CancellationToken cancellationToken)
    {
        var ejecucion = new TblEjecucione
        {
            IdInstancia = command.Request.IdInstancia,
            HoraInicioEjecucion = command.Request.HoraInicioEjecucion,
            HoraFinEjecucion = command.Request.HoraFinEjecucion,
            Descripcion = command.Request.Descripcion
        };

        _unitOfWork.Ejecuciones.Add(ejecucion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ejecucion.IdEjecucion;
    }
}