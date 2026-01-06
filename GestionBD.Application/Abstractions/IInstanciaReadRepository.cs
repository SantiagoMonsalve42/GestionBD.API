using GestionBD.Application.Contracts.Instancias;

namespace GestionBD.Application.Abstractions;

public interface IInstanciaReadRepository : IReadRepository<InstanciaResponse>
{
    Task<InstanciaConnectResponse?> GetConnectionDetailsByEntregableIdAsync(decimal id, CancellationToken cancellationToken = default);
}