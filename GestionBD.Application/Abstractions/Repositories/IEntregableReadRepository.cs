using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Abstractions.Readers;

public interface IEntregableReadRepository : IReadRepository<EntregableResponse>
{
    Task<int> GetEntregablesByEjecucion(decimal idEjecucion);
    Task<IEnumerable<EntregableResponseEstado>> GetAllByIdEjecucionAsync(decimal idEjecucion,CancellationToken cancellationToken = default);
}