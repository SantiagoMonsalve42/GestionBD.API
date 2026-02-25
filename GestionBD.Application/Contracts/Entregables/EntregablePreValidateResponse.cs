namespace GestionBD.Application.Contracts.Entregables;

public sealed record EntregablePreValidateResponse(
    bool IsValid,
    string Script,
    string Status,
    string? Message,
    string? AdditionalInfo
);