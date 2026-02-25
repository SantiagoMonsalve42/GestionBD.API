using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Instancias;

public sealed record CreateInstanciaRequest(
    [Required]
    decimal IdMotor,

    [Required(ErrorMessage = "El nombre de la instancia es requerido")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    string Instancia,

    [Required]
    [Range(1, 65535, ErrorMessage = "El puerto debe estar entre 1 y 65535")]
    int Puerto,

    [Required(ErrorMessage = "El usuario es requerido")]
    [MaxLength(150, ErrorMessage = "El usuario no puede exceder 150 caracteres")]
    string Usuario,

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MaxLength(150, ErrorMessage = "La contraseña no puede exceder 150 caracteres")]
    string Password,
    [Required(ErrorMessage = "El nombre de la base de datos es requerida")]
    [MaxLength(150, ErrorMessage = "El nombre de la base de datos no puede exceder 150 caracteres")]
    string nombreBD
);