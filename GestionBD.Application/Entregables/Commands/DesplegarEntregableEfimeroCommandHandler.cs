using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Domain;
using GestionBD.Domain.Exceptions;
using MediatR;

namespace GestionBD.Application.Entregables.Commands;

public sealed class DesplegarEntregableEfimeroCommandHandler: IRequestHandler<DesplegarEntregableEfimeroCommand, IEnumerable<EntregablePreValidateResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntregableReadRepository _entregableReadRepository;
    private readonly IArtefactoReadRepository _artefactoReadRepository;

    public DesplegarEntregableEfimeroCommandHandler(IUnitOfWork unitOfWork,
                                                    IEntregableReadRepository entregableReadRepository,
                                                    IArtefactoReadRepository artefactoReadRepository)
    {
        _unitOfWork = unitOfWork;
        _entregableReadRepository = entregableReadRepository;
        _artefactoReadRepository = artefactoReadRepository;
    }

    public async Task<IEnumerable<EntregablePreValidateResponse>> Handle(DesplegarEntregableEfimeroCommand request, CancellationToken cancellationToken)
    {
        var entregable=await _entregableReadRepository.GetByIdAsync(request.idEntregable, cancellationToken);
        if (entregable is null)
        {
            throw new ValidationException("Entregable",$"El entregable con Id {request.idEntregable} no existe.");
        }
        var artefactos=await _artefactoReadRepository.GetByEntregableIdAsync(request.idEntregable, cancellationToken);
        
        return new List<EntregablePreValidateResponse>();
    }
}
