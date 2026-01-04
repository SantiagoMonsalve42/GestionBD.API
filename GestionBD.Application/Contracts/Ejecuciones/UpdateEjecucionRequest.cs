using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Ejecuciones;

public sealed record UpdateEjecucionRequest(
    [Required]
    decimal IdEjecucion,
    
    [Required]
    decimal IdInstancia,
    
    DateTime? HoraInicioEjecucion,
    
    DateTime? HoraFinEjecucion,
    
    [MaxLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
    string? Descripcion,

    [Required]
    [MaxLength(150, ErrorMessage = "El nombre del requerimiento no puede exceder 150 caracteres")]
    string NombreRequerimiento
);