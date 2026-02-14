using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;

namespace GestionBD.Application.Abstractions.Repositories.Command;

public interface IEntregableRepository : IRepository<TblEntregable>
{
    Task<bool> removeDatabase(decimal idEntregable,CancellationToken cancellationToken = default);
    Task<bool> UpdateDACPAC(decimal idEntregable, string rutaDacpac,string temporalBD, CancellationToken cancellationToken = default);
    Task<bool> UpdateEstado(decimal idEntregable, EstadoEntregaEnum estadoEntregaEnum, CancellationToken cancellationToken = default);
    Task<bool> UpdateRutaResultado(decimal idEntregable, string rutaResultado, CancellationToken cancellationToken = default);
    Task<bool> UpdateRutaRollback(decimal idEntregable, string rutaRollback, CancellationToken cancellationToken = default);
}