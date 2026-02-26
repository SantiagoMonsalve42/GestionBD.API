namespace GestionBD.Domain.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "No autorizado para realizar esta operación.")
        : base(message)
    {
    }
}