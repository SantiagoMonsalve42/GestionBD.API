namespace GestionBD.Application.Contracts.Motores;

public sealed record MotorResponse(
    decimal IdMotor,
    string NombreMotor,
    string? VersionMotor,
    string? DescripcionMotor
);