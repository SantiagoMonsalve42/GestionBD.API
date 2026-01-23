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

public sealed record EntregableResponseEstado(
    decimal IdEntregable,
    string RutaEntregable,
    string DescripcionEntregable,
    decimal IdEjecucion,
    int NumeroEntrega,
    string? RutaDACPAC,
    string? TemporalBD,
    string EstadoEntrega,
    int EstadoEntregaId,
    string? RutaResultado
);