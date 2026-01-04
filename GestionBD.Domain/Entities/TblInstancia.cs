namespace GestionBD.Domain.Entities;

public partial class TblInstancia
{
    public decimal IdInstancia { get; set; }

    public string Instancia { get; set; } = null!;

    public string Usuario { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal IdMotor { get; set; }

    public int Puerto { get; set; }

    public virtual TblMotore IdMotorNavigation { get; set; } = null!;

    public virtual ICollection<TblEjecucione> TblEjecuciones { get; set; } = new List<TblEjecucione>();
}
