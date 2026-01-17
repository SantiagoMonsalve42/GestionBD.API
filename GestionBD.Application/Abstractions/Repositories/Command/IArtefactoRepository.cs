using GestionBD.Domain.Entities;

namespace GestionBD.Application.Abstractions.Repositories.Command;

public interface IArtefactoRepository : IRepository<TblArtefacto>
{
    Task<bool> UpdateOrder(Dictionary<decimal, int> listadi);
}