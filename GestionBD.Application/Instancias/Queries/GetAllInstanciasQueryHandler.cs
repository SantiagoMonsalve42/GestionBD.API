using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Instancias;

namespace GestionBD.Application.Instancias.Queries;

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