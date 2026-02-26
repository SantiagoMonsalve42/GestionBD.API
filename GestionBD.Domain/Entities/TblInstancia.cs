namespace GestionBD.Domain.Entities;

public partial class TblInstancia
{
    public decimal IdInstancia { get; set; }

    public string Instancia { get; set; } = null!;

    public string? SessionPath { get; set; }

    public decimal IdMotor { get; set; }

    public int Puerto { get; set; }
    public string NombreDB { get; set; } = null!;

    public virtual TblMotore IdMotorNavigation { get; set; } = null!;

    public virtual ICollection<TblEjecucione> TblEjecuciones { get; set; } = new List<TblEjecucione>();
}
