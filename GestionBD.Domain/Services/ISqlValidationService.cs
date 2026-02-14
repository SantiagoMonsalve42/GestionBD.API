using GestionBD.Domain.ValueObjects;

namespace GestionBD.Domain.Services;

public interface ISqlValidationService
{
    Task<SqlValidation> ValidateScriptAsync(
        bool isSecuencial,
        string sqlScript, 
        CancellationToken cancellationToken = default);
}