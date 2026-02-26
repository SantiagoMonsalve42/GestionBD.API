namespace GestionBD.Domain.Entities;

public partial class TblEjecucione
{
    public decimal IdEjecucion { get; set; }

    public decimal IdInstancia { get; set; }

    public DateTime? HoraInicioEjecucion { get; set; }

    public DateTime? HoraFinEjecucion { get; set; }

    public string? Descripcion { get; set; }
    public string? NombreRequerimiento { get; set; }

    public virtual TblInstancia IdInstanciaNavigation { get; set; } = null!;

    public virtual ICollection<TblEntregable> TblEntregables { get; set; } = new List<TblEntregable>();

}
