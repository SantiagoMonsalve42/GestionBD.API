using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories;

public sealed class LogEventoRepository : Repository<TblLogEvento>, ILogEventoRepository
{
    public LogEventoRepository(ApplicationDbContext context) : base(context)
    {
    }
}