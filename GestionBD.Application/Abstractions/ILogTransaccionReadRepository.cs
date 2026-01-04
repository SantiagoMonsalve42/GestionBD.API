using GestionBD.Application.Contracts.LogTransacciones;

namespace GestionBD.Application.Abstractions;

public interface ILogTransaccionReadRepository : IReadRepository<LogTransaccionResponse>
{
}