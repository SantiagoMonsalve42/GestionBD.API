namespace GestionBD.Application.Contracts.Entregables;

public sealed record EntregableResponse(
    decimal IdEntregable,
    string RutaEntregable,
    string DescripcionEntregable,
    decimal IdEjecucion,
    int NumeroEntrega,
    string? RutaDACPAC,
    string? TemporalBD
);