using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Infraestructure.Repositories.Command;

public sealed class EntregableRepository : Repository<TblEntregable>, IEntregableRepository
{
    public EntregableRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateDACPAC(decimal idEntregable, string rutaDacpac,string temporalBD, CancellationToken cancellationToken = default)
    {
        var result = await _context.TblEntregables.Where(x=> x.IdEntregable == idEntregable)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(b => b.RutaDACPAC, rutaDacpac)
                .SetProperty(c => c.TemporalBD, temporalBD), cancellationToken);
        return result > 0;
    }

    public async Task<bool> UpdateEstado(decimal idEntregable, EstadoEntregaEnum estadoEntregaEnum, CancellationToken cancellationToken = default)
    {
        var entregable = await _context.TblEntregables.FirstOrDefaultAsync(x => x.IdEntregable == idEntregable);
        entregable.IdEstado = (decimal)estadoEntregaEnum;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}