using GestionBD.Domain.ValueObjects;

namespace GestionBD.Domain.Services;

/// <summary>
/// Servicio de dominio para generar scripts de rollback
/// </summary>
public interface IRollbackGenerationService
{
    Task<RollbackGeneration> GenerateRollbackAsync(
        string newObjectsDefinitions,
        string currentObjectsDefinitions,
        CancellationToken cancellationToken = default);
}