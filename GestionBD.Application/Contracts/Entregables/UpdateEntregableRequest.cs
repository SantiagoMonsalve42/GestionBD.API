using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Entregables;

public sealed record UpdateEntregableRequest(
    [Required]
    decimal IdEntregable,
    
    [Required(ErrorMessage = "La ruta del entregable es requerida")]
    [MaxLength(150, ErrorMessage = "La ruta no puede exceder 150 caracteres")]
    string RutaEntregable,
    
    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
    string DescripcionEntregable,
    
    [Required]
    decimal IdEjecucion
);