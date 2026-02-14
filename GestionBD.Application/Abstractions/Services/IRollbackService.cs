using GestionBD.Domain.ValueObjects;

namespace GestionBD.Application.Abstractions.Services
{
    public interface IRollbackService
    {
        Task<string> GenerateRollbackScriptAsync(List<RollbackGeneration> rollbackGenerations,string? currentPath, CancellationToken cancellationToken);
    }
}
