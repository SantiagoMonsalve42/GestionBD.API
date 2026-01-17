using GestionBD.Domain.Entities;
using GestionBD.Domain.Enum;

namespace GestionBD.Application.Abstractions.Repositories.Command;

public interface IEntregableRepository : IRepository<TblEntregable>
{
    Task<bool> UpdateDACPAC(decimal idEntregable, string rutaDacpac,string temporalBD, CancellationToken cancellationToken = default);
    Task<bool> UpdateEstado(decimal idEntregable, EstadoEntregaEnum estadoEntregaEnum, CancellationToken cancellationToken = default);
}