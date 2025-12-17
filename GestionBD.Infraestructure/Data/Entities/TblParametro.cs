using System;
using System.Collections.Generic;

namespace GestionBD.Infraestructure.Data.Entities;

public partial class TblParametro
{
    public decimal IdParametro { get; set; }

    public string NombreParametro { get; set; } = null!;

    public decimal? ValorNumerico { get; set; }

    public string? ValorString { get; set; }
}
