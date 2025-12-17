using System;
using System.Collections.Generic;

namespace GestionBD.Infraestructure.Data.Entities;

public partial class TblLogTransaccione
{
    public decimal IdTransaccion { get; set; }

    public string NombreTransaccion { get; set; } = null!;

    public string EstadoTransaccion { get; set; } = null!;

    public string DescripcionTransaccion { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public string? RespuestaTransaccion { get; set; }

    public DateTime? FechaFin { get; set; }

    public string UsuarioEjecucion { get; set; } = null!;

    public virtual ICollection<TblLogEvento> TblLogEventos { get; set; } = new List<TblLogEvento>();
}
