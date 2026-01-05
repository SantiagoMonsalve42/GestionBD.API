namespace GestionBD.Application.Contracts.Entregables;

public sealed record EntregableResponse(
    decimal IdEntregable,
    string RutaEntregable,
    string DescripcionEntregable,
    decimal IdEjecucion,
    string? NombreRequerimiento,
    int NumeroEntrega
);