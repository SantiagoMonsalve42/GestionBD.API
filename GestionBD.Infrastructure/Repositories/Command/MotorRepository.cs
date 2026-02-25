using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Domain.Entities;
using GestionBD.Infrastructure.Data;

namespace GestionBD.Infrastructure.Repositories.Command;

public sealed class MotorRepository : Repository<TblMotore>, IMotorRepository
{
    public MotorRepository(ApplicationDbContext context) : base(context)
    {
    }
}