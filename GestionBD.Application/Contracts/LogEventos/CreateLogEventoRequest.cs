using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.LogEventos;

public sealed record CreateLogEventoRequest(
    [Required]
    decimal IdTransaccion,
    
    [Required]
    DateTime FechaEjecucion,
    
    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    string Descripcion,
    
    [Required(ErrorMessage = "El estado del evento es requerido")]
    [MaxLength(500, ErrorMessage = "El estado no puede exceder 500 caracteres")]
    string EstadoEvento
);