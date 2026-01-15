using System.ComponentModel.DataAnnotations;

namespace GestionBD.Application.Contracts.Artefactos;

public sealed record ArtefactoChangeOrder(
    [Required]
    decimal IdArtefacto,

    [Required]
    int OrdenEjecucion
);
