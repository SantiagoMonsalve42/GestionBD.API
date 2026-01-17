using MediatR;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Instancias.Queries;

namespace GestionBD.Application.Instancias.QueriesHandlers;

public sealed class GetAllInstanciasQueryHandler : IRequestHandler<GetAllInstanciasQuery, IEnumerable<InstanciaResponse>>
{
    private readonly IInstanciaReadRepository _repository;

    public GetAllInstanciasQueryHandler(IInstanciaReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<InstanciaResponse>> Handle(GetAllInstanciasQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}