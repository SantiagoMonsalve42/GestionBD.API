using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories;

public sealed class EntregableRepository : Repository<TblEntregable>, IEntregableRepository
{
    public EntregableRepository(ApplicationDbContext context) : base(context)
    {
    }
}