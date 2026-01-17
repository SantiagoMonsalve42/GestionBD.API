using GestionBD.Application.Contracts.Instancias;

namespace GestionBD.Application.Abstractions.Repositories.Query;

public interface IInstanciaReadRepository : IReadRepository<InstanciaResponse>
{
    Task<InstanciaConnectResponse?> GetConnectionDetailsByEntregableIdAsync(decimal id, CancellationToken cancellationToken = default);
}