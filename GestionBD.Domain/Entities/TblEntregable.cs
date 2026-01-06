namespace GestionBD.Domain.Entities;

public partial class TblEntregable
{
    public decimal IdEntregable { get; set; }

    public string RutaEntregable { get; set; } = null!;

    public string DescripcionEntregable { get; set; } = null!;

    public decimal IdEjecucion { get; set; }
    public int NumeroEntrega { get; set; }

    public string? RutaDACPAC { get; set; } 

    public virtual TblEjecucione IdEjecucionNavigation { get; set; } = null!;

    public virtual ICollection<TblArtefacto> TblArtefactos { get; set; } = new List<TblArtefacto>();
}
