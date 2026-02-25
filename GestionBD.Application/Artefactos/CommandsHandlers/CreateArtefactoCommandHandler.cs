using GestionBD.Application.Artefactos.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Artefactos.CommandsHandlers;

public sealed class CreateArtefactoCommandHandler : IRequestHandler<CreateArtefactoCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateArtefactoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateArtefactoCommand command, CancellationToken cancellationToken)
    {
        var artefacto = new TblArtefacto
        {
            IdEntregable = command.Request.IdEntregable,
            OrdenEjecucion = command.Request.OrdenEjecucion,
            Codificacion = command.Request.Codificacion,
            NombreArtefacto = command.Request.NombreArtefacto,
            RutaRelativa = command.Request.RutaRelativa,
            EsReverso = command.Request.EsReverso
        };

        _unitOfWork.Artefactos.Add(artefacto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return artefacto.IdArtefacto;
    }
}