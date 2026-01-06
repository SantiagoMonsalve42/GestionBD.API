namespace GestionBD.Application.Contracts.Instancias;

public sealed record InstanciaConnectResponse(
    string Instancia,
    string Usuario,
    string Password,
    int Puerto,
    string NombreBD
);