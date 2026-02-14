using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Abstractions.Repositories.Query;

public interface IEntregableReadRepository : IReadRepository<EntregableResponseEstado>
{
    Task<int> GetEntregablesByEjecucion(decimal idEjecucion);
    Task<IEnumerable<EntregableResponseEstado>> GetAllByIdEjecucionAsync(decimal idEjecucion,CancellationToken cancellationToken = default);
    Task<IEnumerable<EntregableRevisionResponse>> GetAllRevisionesAsync(CancellationToken cancellationToken = default);
}