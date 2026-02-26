namespace GestionBD.Application.Contracts.Instancias;

public sealed record InstanciaResponse(
    decimal IdInstancia,
    decimal IdMotor,
    string Instancia,
    int Puerto,
    string NombreBD
);
