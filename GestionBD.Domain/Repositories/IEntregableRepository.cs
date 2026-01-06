using GestionBD.Domain.Entities;

namespace GestionBD.Domain.Repositories;

public interface IEntregableRepository : IRepository<TblEntregable>
{
    Task<bool> UpdateDACPAC(decimal idEntregable, string rutaDacpac, CancellationToken cancellationToken = default);
}