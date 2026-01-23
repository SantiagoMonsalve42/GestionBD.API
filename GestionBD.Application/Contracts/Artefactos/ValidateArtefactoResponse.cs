using GestionBD.Domain.ValueObjects;

namespace GestionBD.Application.Contracts.Artefactos;

public sealed record ValidateArtefactoResponse(
    string Name,
    SqlValidation SqlValidation
);
