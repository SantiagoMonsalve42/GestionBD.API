namespace GestionBD.Domain.Entities;

public partial class TblArtefacto
{
    public decimal IdArtefacto { get; set; }

    public decimal IdEntregable { get; set; }

    public int OrdenEjecucion { get; set; }

    public string Codificacion { get; set; } = null!;

    public string NombreArtefacto { get; set; } = null!;

    public string RutaRelativa { get; set; } = null!;

    public bool EsReverso { get; set; }

    public virtual TblEntregable IdEntregableNavigation { get; set; } = null!;
}
