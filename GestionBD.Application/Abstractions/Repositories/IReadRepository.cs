namespace GestionBD.Application.Abstractions.Readers;

public interface IReadRepository<TResponse> where TResponse : class
{
    Task<IEnumerable<TResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default);
}