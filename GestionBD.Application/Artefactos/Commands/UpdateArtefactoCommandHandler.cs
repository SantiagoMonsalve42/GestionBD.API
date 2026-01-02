using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Artefactos.Commands;

public sealed class UpdateArtefactoCommandHandler : IRequestHandler<UpdateArtefactoCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateArtefactoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateArtefactoCommand command, CancellationToken cancellationToken)
    {
        var artefacto = await _context.TblArtefactos
            .FirstOrDefaultAsync(a => a.IdArtefacto == command.Request.IdArtefacto, cancellationToken);

        if (artefacto == null)
            throw new KeyNotFoundException($"Artefacto con ID {command.Request.IdArtefacto} no encontrado.");

        artefacto.IdEntregable = command.Request.IdEntregable;
        artefacto.OrdenEjecucion = command.Request.OrdenEjecucion;
        artefacto.Codificacion = command.Request.Codificacion;
        artefacto.NombreArtefacto = command.Request.NombreArtefacto;
        artefacto.RutaRelativa = command.Request.RutaRelativa;
        artefacto.EsReverso = command.Request.EsReverso;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}