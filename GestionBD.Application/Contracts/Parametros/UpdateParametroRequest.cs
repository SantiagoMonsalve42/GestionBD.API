using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Parametros;

public sealed record UpdateParametroRequest(
    [Required]
    decimal IdParametro,
    
    [Required(ErrorMessage = "El nombre del parámetro es requerido")]
    [MaxLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
    string NombreParametro,
    
    decimal? ValorNumerico,
    
    [MaxLength(150, ErrorMessage = "El valor string no puede exceder 150 caracteres")]
    string? ValorString
);