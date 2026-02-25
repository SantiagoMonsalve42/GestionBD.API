using GestionBD.Domain;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Infrastructure.Data;
using GestionBD.Infrastructure.Repositories.Command;
using Microsoft.EntityFrameworkCore.Storage;

namespace GestionBD.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IMotorRepository? _motores;
    private IInstanciaRepository? _instancias;
    private IEjecucionRepository? _ejecuciones;
    private IEntregableRepository? _entregables;
    private IArtefactoRepository? _artefactos;
    private ILogTransaccionRepository? _logTransacciones;
    private ILogEventoRepository? _logEventos;
    private IParametroRepository? _parametros;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IMotorRepository Motores => _motores ??= new MotorRepository(_context);
    public IInstanciaRepository Instancias => _instancias ??= new InstanciaRepository(_context);
    public IEjecucionRepository Ejecuciones => _ejecuciones ??= new EjecucionRepository(_context);
    public IEntregableRepository Entregables => _entregables ??= new EntregableRepository(_context);
    public IArtefactoRepository Artefactos => _artefactos ??= new ArtefactoRepository(_context);
    public ILogTransaccionRepository LogTransacciones => _logTransacciones ??= new LogTransaccionRepository(_context);
    public ILogEventoRepository LogEventos => _logEventos ??= new LogEventoRepository(_context);
    public IParametroRepository Parametros => _parametros ??= new ParametroRepository(_context);

    public async Task<TEntity?> FindEntityAsync<TEntity>(decimal id, CancellationToken cancellationToken = default) where TEntity : class
    {
        return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}