using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;

namespace GestionBD.Infrastructure.Repositories.Command;

public sealed class InstanciaRepository : Repository<TblInstancia>, IInstanciaRepository
{
    public InstanciaRepository(ApplicationDbContext context) : base(context)
    {
    }
}