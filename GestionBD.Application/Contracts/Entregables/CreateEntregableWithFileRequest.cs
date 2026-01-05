using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestionBD.Application.Contracts.Entregables;

public sealed class CreateEntregableWithFileRequest
{
    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
    public string DescripcionEntregable { get; set; } = null!;
    
    [Required(ErrorMessage = "El ID de ejecución es requerido")]
    public decimal IdEjecucion { get; set; }

    [Required(ErrorMessage = "El número de entrega es requerido")]
    public int NumeroEntrega { get; set; }

    [Required(ErrorMessage = "El archivo es requerido")]
    public IFormFile File { get; set; } = null!;
}