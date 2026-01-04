using GestionBD.Domain.Repositories;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Infraestructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}