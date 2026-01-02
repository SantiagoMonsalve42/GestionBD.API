using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.Artefactos.Commands;

public sealed class CreateArtefactoCommandHandler : IRequestHandler<CreateArtefactoCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateArtefactoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
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

        _context.TblArtefactos.Add(artefacto);
        await _context.SaveChangesAsync(cancellationToken);

        return artefacto.IdArtefacto;
    }
}