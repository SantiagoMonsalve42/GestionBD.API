using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;

namespace GestionBD.Infrastructure.Repositories.Command;

public sealed class LogTransaccionRepository : Repository<TblLogTransaccione>, ILogTransaccionRepository
{
    public LogTransaccionRepository(ApplicationDbContext context) : base(context)
    {
    }
}