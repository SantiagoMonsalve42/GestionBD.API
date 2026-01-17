using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Infraestructure.Repositories.Command;

public sealed class ArtefactoRepository : Repository<TblArtefacto>, IArtefactoRepository
{
    public ArtefactoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> UpdateOrder(Dictionary<decimal, int> listado)
    {
        foreach (var item in listado)
        {
            var artefacto = await _context.TblArtefactos.FirstOrDefaultAsync(x => x.IdArtefacto == item.Key);
            if (artefacto == null) 
                throw new KeyNotFoundException("El artefacto no existe");                
            artefacto.OrdenEjecucion = item.Value;
            _context.TblArtefactos.Update(artefacto);
            await _context.SaveChangesAsync();
        }
        return true;
    }
}