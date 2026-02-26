using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Queries;
using MediatR;

namespace GestionBD.Application.Instancias.QueriesHandlers;

public sealed class GetInstanciaByIdQueryHandler : IRequestHandler<GetInstanciaByIdQuery, InstanciaResponse?>
{
    private readonly IInstanciaReadRepository _repository;

    public GetInstanciaByIdQueryHandler(IInstanciaReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<InstanciaResponse?> Handle(GetInstanciaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdInstancia, cancellationToken);
    }
}