using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.Application.Ejecuciones.Queries;

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