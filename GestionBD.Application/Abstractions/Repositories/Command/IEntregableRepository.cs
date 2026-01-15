using GestionBD.Domain.Entities;

namespace GestionBD.Application.Abstractions.Repositories.Command;

public interface IEntregableRepository : IRepository<TblEntregable>
{
    Task<bool> UpdateDACPAC(decimal idEntregable, string rutaDacpac,string temporalBD, CancellationToken cancellationToken = default);
}