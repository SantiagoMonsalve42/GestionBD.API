using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories.Command;

public sealed class InstanciaRepository : Repository<TblInstancia>, IInstanciaRepository
{
    public InstanciaRepository(ApplicationDbContext context) : base(context)
    {
    }
}