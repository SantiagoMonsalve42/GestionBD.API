using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;

namespace GestionBD.Infrastructure.Repositories.Command;

public sealed class EjecucionRepository : Repository<TblEjecucione>, IEjecucionRepository
{
    public EjecucionRepository(ApplicationDbContext context) : base(context)
    {
    }
}