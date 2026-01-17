using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Application.Abstractions.Repositories.Query;

public interface IEjecucionReadRepository : IReadRepository<EjecucionResponse>
{
    Task<bool> ExistsByReqName(string reqName, CancellationToken cancellationToken = default);
}