using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.Abstractions;
using GestionBD.Domain.Exceptions;

namespace GestionBD.Application.Ejecuciones.Commands;

public sealed class CreateEjecucionCommandHandler : IRequestHandler<CreateEjecucionCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEjecucionReadRepository _ejecucionReadRepository;

    public CreateEjecucionCommandHandler(IUnitOfWork unitOfWork,IEjecucionReadRepository ejecucionReadRepository)
    {
        _unitOfWork = unitOfWork;
        _ejecucionReadRepository = ejecucionReadRepository;
    }

    public async Task<decimal> Handle(CreateEjecucionCommand command, CancellationToken cancellationToken)
    {

        var ejecucion = new TblEjecucione
        {
            IdInstancia = command.Request.IdInstancia,
            HoraInicioEjecucion = DateTime.Now,
            HoraFinEjecucion = null,
            Descripcion = command.Request.Descripcion,
            NombreRequerimiento = command.Request.NombreRequerimiento
        };
        if(await _ejecucionReadRepository.ExistsByReqName(command.Request.NombreRequerimiento))
        {
            throw new ValidationException("NombreRequerimiento", "Ya existe una ejecución con el mismo nombre de requerimiento.");
        }
        _unitOfWork.Ejecuciones.Add(ejecucion);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ejecucion.IdEjecucion;
    }
}