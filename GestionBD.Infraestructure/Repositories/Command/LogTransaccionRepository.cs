using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories.Command;

public sealed class LogTransaccionRepository : Repository<TblLogTransaccione>, ILogTransaccionRepository
{
    public LogTransaccionRepository(ApplicationDbContext context) : base(context)
    {
    }
}