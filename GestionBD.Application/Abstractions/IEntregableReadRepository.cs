using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.Application.Abstractions;

public interface IEntregableReadRepository : IReadRepository<EntregableResponse>
{
    Task<int> GetEntregablesByEjecucion(decimal idEjecucion);
}