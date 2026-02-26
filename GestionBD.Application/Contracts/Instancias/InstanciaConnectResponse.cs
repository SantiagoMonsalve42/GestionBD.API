namespace GestionBD.Application.Contracts.Instancias;

public sealed record InstanciaConnectResponse(
    string Instancia,
    string SessionPath,
    int Puerto,
    string NombreBD,
    string? TemporalBD
);