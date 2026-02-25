using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;

namespace GestionBD.Infrastructure.Repositories.Command;

public sealed class LogEventoRepository : Repository<TblLogEvento>, ILogEventoRepository
{
    public LogEventoRepository(ApplicationDbContext context) : base(context)
    {
    }
}