using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories;

public sealed class ParametroRepository : Repository<TblParametro>, IParametroRepository
{
    public ParametroRepository(ApplicationDbContext context) : base(context)
    {
    }
}