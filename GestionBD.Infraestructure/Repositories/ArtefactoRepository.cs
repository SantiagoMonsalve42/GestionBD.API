using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories;

public sealed class ArtefactoRepository : Repository<TblArtefacto>, IArtefactoRepository
{
    public ArtefactoRepository(ApplicationDbContext context) : base(context)
    {
    }
}