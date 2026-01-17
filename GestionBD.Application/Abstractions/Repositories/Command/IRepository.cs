namespace GestionBD.Application.Abstractions.Repositories.Command;
public interface IRepository<TEntity> where TEntity : class
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}