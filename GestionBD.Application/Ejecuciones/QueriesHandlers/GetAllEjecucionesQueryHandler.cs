using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Ejecuciones.Queries;

namespace GestionBD.Application.Ejecuciones.QueriesHandlers;

public sealed class GetAllEjecucionesQueryHandler : IRequestHandler<GetAllEjecucionesQuery, IEnumerable<EjecucionResponse>>
{
    private readonly IEjecucionReadRepository _repository;

    public GetAllEjecucionesQueryHandler(IEjecucionReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EjecucionResponse>> Handle(GetAllEjecucionesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}