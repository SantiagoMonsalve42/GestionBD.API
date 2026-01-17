using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Ejecuciones;

public sealed record UpdateEjecucionRequest(
    [Required]
    decimal IdEjecucion,
    
    [Required]
    decimal IdInstancia,
    
    [MaxLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
    string? Descripcion
);