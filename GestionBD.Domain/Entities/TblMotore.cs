namespace GestionBD.Domain.Entities;

public partial class TblMotore
{
    public decimal IdMotor { get; set; }

    public string NombreMotor { get; set; } = null!;

    public string VersionMotor { get; set; } = null!;

    public string DescripcionMotor { get; set; } = null!;

    public virtual ICollection<TblInstancia> TblInstancia { get; set; } = new List<TblInstancia>();
}
