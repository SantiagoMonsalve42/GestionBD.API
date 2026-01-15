using MediatR;
using GestionBD.Application.Contracts.Ejecuciones;
using GestionBD.Application.Abstractions.Repositories.Query;

namespace GestionBD.Application.Ejecuciones.Queries;

public sealed class GetEjecucionByIdQueryHandler : IRequestHandler<GetEjecucionByIdQuery, EjecucionResponse?>
{
    private readonly IEjecucionReadRepository _repository;

    public GetEjecucionByIdQueryHandler(IEjecucionReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<EjecucionResponse?> Handle(GetEjecucionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdEjecucion, cancellationToken);
    }
}