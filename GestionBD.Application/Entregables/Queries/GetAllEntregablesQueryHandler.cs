using MediatR;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Abstractions.Readers;

namespace GestionBD.Application.Entregables.Queries;

public sealed class GetAllEntregablesQueryHandler : IRequestHandler<GetAllEntregablesQuery, IEnumerable<EntregableResponse>>
{
    private readonly IEntregableReadRepository _repository;

    public GetAllEntregablesQueryHandler(IEntregableReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EntregableResponse>> Handle(GetAllEntregablesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}