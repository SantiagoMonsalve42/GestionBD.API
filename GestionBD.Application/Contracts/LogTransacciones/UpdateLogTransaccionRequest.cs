using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.LogTransacciones;

public sealed record UpdateLogTransaccionRequest(
    [Required]
    decimal IdTransaccion,

    [Required(ErrorMessage = "El nombre de la transacción es requerido")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    string NombreTransaccion,

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(1, ErrorMessage = "El estado debe ser de 1 carácter")]
    string EstadoTransaccion,

    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
    string DescripcionTransaccion,

    [Required]
    DateTime FechaInicio,

    [MaxLength(500, ErrorMessage = "La respuesta no puede exceder 500 caracteres")]
    string? RespuestaTransaccion,

    DateTime? FechaFin,

    [Required(ErrorMessage = "El usuario de ejecución es requerido")]
    [MaxLength(150, ErrorMessage = "El usuario no puede exceder 150 caracteres")]
    string UsuarioEjecucion
);