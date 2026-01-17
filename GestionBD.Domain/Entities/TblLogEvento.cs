namespace GestionBD.Domain.Entities;

public partial class TblLogEvento
{
    public decimal IdEvento { get; set; }

    public decimal IdTransaccion { get; set; }

    public DateTime FechaEjecucion { get; set; }

    public string Descripcion { get; set; } = null!;

    public string EstadoEvento { get; set; } = null!;

    public virtual TblLogTransaccione IdTransaccionNavigation { get; set; } = null!;
}
