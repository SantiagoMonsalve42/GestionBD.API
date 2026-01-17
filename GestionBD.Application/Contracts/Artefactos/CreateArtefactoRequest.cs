using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Artefactos;

public sealed record CreateArtefactoRequest(
    [Required]
    decimal IdEntregable,
    
    [Required]
    int OrdenEjecucion,
    
    [Required(ErrorMessage = "La codificación es requerida")]
    [MaxLength(50, ErrorMessage = "La codificación no puede exceder 50 caracteres")]
    string Codificacion,
    
    [Required(ErrorMessage = "El nombre del artefacto es requerido")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    string NombreArtefacto,
    
    [Required(ErrorMessage = "La ruta relativa es requerida")]
    [MaxLength(150, ErrorMessage = "La ruta no puede exceder 150 caracteres")]
    string RutaRelativa,
    
    [Required]
    bool EsReverso
);