using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories.Command;

public sealed class EjecucionRepository : Repository<TblEjecucione>, IEjecucionRepository
{
    public EjecucionRepository(ApplicationDbContext context) : base(context)
    {
    }
}