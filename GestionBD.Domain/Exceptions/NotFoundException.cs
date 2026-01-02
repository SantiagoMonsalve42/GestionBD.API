namespace GestionBD.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"La entidad '{entityName}' con el identificador '{key}' no fue encontrada.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}