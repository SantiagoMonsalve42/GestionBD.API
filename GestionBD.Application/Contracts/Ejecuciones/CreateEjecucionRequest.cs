using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Ejecuciones;

public sealed record CreateEjecucionRequest(
    [Required]
    decimal IdInstancia,
    
    [Required]
    DateTime HoraInicioEjecucion,
    
    [Required]
    DateTime HoraFinEjecucion,
    
    [MaxLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
    string? Descripcion
);