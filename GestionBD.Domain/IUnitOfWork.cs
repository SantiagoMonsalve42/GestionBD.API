using GestionBD.Domain.Repositories;

namespace GestionBD.Domain;

public interface IUnitOfWork : IDisposable
{
    IMotorRepository Motores { get; }
    IInstanciaRepository Instancias { get; }
    IEjecucionRepository Ejecuciones { get; }
    IEntregableRepository Entregables { get; }
    IArtefactoRepository Artefactos { get; }
    ILogTransaccionRepository LogTransacciones { get; }
    ILogEventoRepository LogEventos { get; }
    IParametroRepository Parametros { get; }

    Task<TEntity?> FindEntityAsync<TEntity>(decimal id, CancellationToken cancellationToken = default) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}