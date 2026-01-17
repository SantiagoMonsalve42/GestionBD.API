using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Motores;

public sealed record CreateMotorRequest(
    [Required(ErrorMessage = "El nombre del motor es requerido")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    string NombreMotor,
    
    [MaxLength(150, ErrorMessage = "La versión no puede exceder 150 caracteres")]
    string? VersionMotor,
    
    [MaxLength(50, ErrorMessage = "La descripción no puede exceder 50 caracteres")]
    string? DescripcionMotor
);