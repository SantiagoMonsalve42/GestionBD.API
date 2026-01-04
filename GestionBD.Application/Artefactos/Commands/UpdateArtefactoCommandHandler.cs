using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Artefactos.Commands;

public sealed class UpdateArtefactoCommandHandler : IRequestHandler<UpdateArtefactoCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateArtefactoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateArtefactoCommand command, CancellationToken cancellationToken)
    {
        var artefacto = await _unitOfWork.FindEntityAsync<TblArtefacto>(command.Request.IdArtefacto, cancellationToken);

        if (artefacto == null)
            throw new KeyNotFoundException($"Artefacto con ID {command.Request.IdArtefacto} no encontrado.");

        artefacto.IdEntregable = command.Request.IdEntregable;
        artefacto.OrdenEjecucion = command.Request.OrdenEjecucion;
        artefacto.Codificacion = command.Request.Codificacion;
        artefacto.NombreArtefacto = command.Request.NombreArtefacto;
        artefacto.RutaRelativa = command.Request.RutaRelativa;
        artefacto.EsReverso = command.Request.EsReverso;

        _unitOfWork.Artefactos.Update(artefacto);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}