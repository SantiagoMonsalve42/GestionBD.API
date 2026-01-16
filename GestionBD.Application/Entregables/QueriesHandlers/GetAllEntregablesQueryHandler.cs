using MediatR;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Entregables.Queries;

namespace GestionBD.Application.Entregables.QueriesHandlers;

public sealed class GetAllEntregablesQueryHandler : IRequestHandler<GetAllEntregablesQuery, IEnumerable<EntregableResponseEstado>>
{
    private readonly IEntregableReadRepository _repository;

    public GetAllEntregablesQueryHandler(IEntregableReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EntregableResponseEstado>> Handle(GetAllEntregablesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}