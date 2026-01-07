using GestionBD.Application.Contracts.Artefactos;

namespace GestionBD.Application.Abstractions;

public interface IArtefactoReadRepository : IReadRepository<ArtefactoResponse>
{
    Task<IEnumerable<ArtefactoResponse>> GetByEntregableIdAsync(decimal id, CancellationToken cancellationToken = default);
}