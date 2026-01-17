namespace GestionBD.Application.Abstractions.Repositories.Query;

public interface IReadRepository<TResponse> where TResponse : class
{
    Task<IEnumerable<TResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TResponse?> GetByIdAsync(decimal id, CancellationToken cancellationToken = default);
}