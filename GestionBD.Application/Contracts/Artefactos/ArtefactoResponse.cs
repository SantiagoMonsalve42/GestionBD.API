namespace GestionBD.Application.Contracts.Artefactos;

public sealed record ArtefactoResponse(
    decimal IdArtefacto,
    decimal IdEntregable,
    int OrdenEjecucion,
    string Codificacion,
    string NombreArtefacto,
    string RutaRelativa,
    bool EsReverso,
    string? DescripcionEntregable
);