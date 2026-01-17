using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories.Command;

public sealed class LogEventoRepository : Repository<TblLogEvento>, ILogEventoRepository
{
    public LogEventoRepository(ApplicationDbContext context) : base(context)
    {
    }
}