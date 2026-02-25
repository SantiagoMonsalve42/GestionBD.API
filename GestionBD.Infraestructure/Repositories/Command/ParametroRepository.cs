using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;

namespace GestionBD.Infrastructure.Repositories.Command;

public sealed class ParametroRepository : Repository<TblParametro>, IParametroRepository
{
    public ParametroRepository(ApplicationDbContext context) : base(context)
    {
    }
}