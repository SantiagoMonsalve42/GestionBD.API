using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Application.Abstractions;

public interface IEjecucionReadRepository : IReadRepository<EjecucionResponse>
{
    Task<bool> ExistsByReqName(string reqName, CancellationToken cancellationToken = default);
}