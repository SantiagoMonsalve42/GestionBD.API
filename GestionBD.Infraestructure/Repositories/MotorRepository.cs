using GestionBD.Domain.Repositories;
using GestionBD.Domain.Entities;
using GestionBD.Infraestructure.Data;

namespace GestionBD.Infraestructure.Repositories;

public sealed class MotorRepository : Repository<TblMotore>, IMotorRepository
{
    public MotorRepository(ApplicationDbContext context) : base(context)
    {
    }
}